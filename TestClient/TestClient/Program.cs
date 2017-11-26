using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MessagePack;

namespace TestClient
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			var network = new SocketClient();
			network.Init();

			var isConnected = network.ConnectToServer("127.0.0.1", 23452);

			if (isConnected)
			{
				var req = new LoginReq()
				{
					UserId = "Test",
					Token = 1234
				};

				var byteReq = MessagePackSerializer.Serialize(req);

				network.Send(byteReq);
				network.WaitForReceive();
			}

			Console.ReadLine();
			network.Close();
		}
	}

	public class SocketClient
	{
		Socket socket;
		byte[] buffer = new byte[1024];

		public void Init()
		{
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		public bool ConnectToServer(string ip, int port)
		{
			try
			{
				IPAddress ipAddr = System.Net.IPAddress.Parse(ip);
				IPEndPoint iPEndPoint = new System.Net.IPEndPoint(ipAddr, port);
				socket.Connect(iPEndPoint);

				Console.WriteLine("Socket Connect to Server Success");
				return true;
			}
			catch (SocketException e)
			{
				Console.WriteLine($"Socket Exception : {e.Message}");
				return false;
			}
		}

		public void Send(byte[] message)
		{
			var header = new PacketHeader()
			{
				PacketId = 101,
				BodySize = message.Length
			};

			var byteHeader = MessagePackSerializer.Serialize(header);

			byte[] buffer = new byte[byteHeader.Length + message.Length];

			Array.Copy(byteHeader, 0, buffer, 0, byteHeader.Length);
			Array.Copy(message, 0, buffer, byteHeader.Length, message.Length);

			socket.Send(buffer, SocketFlags.None);

			Console.WriteLine($"Send Byte({buffer.Length}), Header Length({byteHeader.Length}), Message Length({message.Length})");
		}

		public void WaitForReceive()
		{
			int receivedSize = 0;

			try
			{
				receivedSize = socket.Receive(buffer, SocketFlags.None);

				PacketHeader header;

				if (receivedSize > 3)
				{
					byte[] headerBytes = new byte[3];

					Array.Copy(buffer, headerBytes, 3);

					header = MessagePackSerializer.Deserialize<PacketHeader>(headerBytes);

				}
				else
				{
					Console.WriteLine("Received size is smaller than header.");
					return;
				}

				if (receivedSize >= header.BodySize + 3)
				{
					byte[] bodyBytes = new byte[header.BodySize];

					Array.Copy(buffer, 3, bodyBytes, 0, header.BodySize);

					var loginRes = MessagePackSerializer.Deserialize<LoginRes>(bodyBytes);

					Console.WriteLine($"loginRes Arrived({loginRes.Result})");
				}
				else
				{
					Console.WriteLine("Received size is smaller than body.");
					return;
				}
			}
			catch (SocketException e)
			{
				Console.WriteLine($"Receive failed. Message({e.Message})");
			}
			finally
			{
				Array.Clear(buffer, 0, 1024);
			}
		}

		internal void Close()
		{
			socket.Close();
		}
	}


	[MessagePackObject]
	public class PacketHeader
	{
		[Key(0)]
		public int PacketId;
		[Key(1)]
		public int BodySize;
	}

	[MessagePackObject]
	public class LoginReq
	{
		[Key(0)]
		public string UserId;
		[Key(1)]
		public Int64 Token;
	}

	[MessagePackObject]
	public class LoginRes
	{
		[Key(0)]
		public int Result;
	}
}
