using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEngine;
using MessagePack;

internal class TcpNetwork
{
	#region NETWORK VARIABLES

	// recv, send가 발생했을 때 불러줄 비동기 콜백
	private AsyncCallback recvCallBack;
    private AsyncCallback sendCallBack;

    private Socket socket;
    private Queue<Packet> packetQueue;

    public bool   IsConnected  { get; private set; }
    public string Ip           { get; private set; }
    public int    Port         { get; private set; }

	#endregion

	public TcpNetwork(string ipAddress = "127.0.0.1", int port = 23452)
	{
		IsConnected = false;
		Ip = ipAddress;
		Port = port;

		recvCallBack = new AsyncCallback(RecvCallBack);
		sendCallBack = new AsyncCallback(SendCallBack);
		packetQueue = new Queue<Packet>();

		try
		{
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}
		catch (Exception e)
		{
			Debug.LogError("Socket creation falied error : " + e.Message);
		}
	}

	public bool IsMessageExist()
	{
		lock (packetQueue)
		{
			if (packetQueue.Count == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}

	public Packet GetPacketFromQueue()
	{
		lock (packetQueue)
		{
			return packetQueue.Dequeue();
		}
	}

	public void CloseNetwork()
	{
		IsConnected = false;
		socket.Shutdown(SocketShutdown.Both);
		socket.Close();
		socket = null;
	}

	// 게임 서버에게 요청한 패킷을 보내주는 메소드.
	public void SendPacket<T>(T data, PacketId packetId)
	{
		if (IsConnected == false)
		{
			Debug.LogAssertion("Socket connection not completed yet");
			return;
		}

		var sendData = new AsyncSendData(socket);

		var messageBytes = MessagePackSerializer.Serialize(data);

		var header = new PacketHeader()
		{
			PacketId = (int)packetId,
			BodySize = messageBytes.Length
		};

		var headerBytes = MessagePackSerializer.Serialize(header);

		sendData.Buffer = new byte[headerBytes.Length + messageBytes.Length];

		Array.Copy(headerBytes, 0, sendData.Buffer, 0, headerBytes.Length);
		Array.Copy(messageBytes, 0, sendData.Buffer, headerBytes.Length, messageBytes.Length);

		try
		{
			// 비동기 전송 시작. 파라미터는 BeginReceive와 대동소이하므로 생략. 
			socket.BeginSend(
				sendData.Buffer,
				0,
				NetworkDefinition.PacketHeaderSize + messageBytes.Length,
				SocketFlags.None,
				sendCallBack,
				sendData);
		}
		catch (SocketException e)
		{
			HandleException(e);
			return;
		}

	}

	// 게임 서버와 접속을 시도하는 메소드.
	public void ConnectToServer()
	{
		try
		{
			socket.BeginConnect(Ip, Port, OnConnectSuccess, 0);
		}
		catch (SocketException e)
		{
			Debug.LogAssertion("Server connection failed : " + e.ToString());
			IsConnected = false;
		}
	}

	// 게임 서버와의 비동기 접속이 끝났을 경우 호출되는 콜백 메소드.
	private void OnConnectSuccess(IAsyncResult asyncResult)
	{
		try
		{
			// 접속이 완료되었으므로 Connect 요청을 그만 둠.
			socket.EndConnect(asyncResult);
			IsConnected = true;
			Debug.LogFormat("Server connect success ip {0}, port {1}", Ip, Port);
		}
		catch (Exception e)
		{
			Debug.Log("Socket connect callback function failed : " + e.Message);
			return;
		}

		var recvData = new AsyncRecvData(NetworkDefinition.BufferSize, socket);

		// 연결된 소켓에 Recv를 걸어준다.
		socket.BeginReceive(
			recvData.Buffer,             // 버퍼
			0,                            // 받은 데이터를 저장할 zero-base postion
			recvData.Buffer.Length,      // 버퍼 길이
			SocketFlags.None,             // SocketFlags, 여기서는 None을 지정한다.
			recvCallBack,                 // Recv IO가 끝난 뒤 호출할 메소드.
			recvData                      // IO가 끝난 뒤 호출할 메소드의 인자로 들어갈 사용자 구조체.
			);
	}

	// Recv IO 작업이 끝났을 때 호출 될 콜백 메소드.
	private void RecvCallBack(IAsyncResult asyncResult)
	{
		if (IsConnected == false)
		{
			Debug.LogAssertion("TcpNetwork was not connected yet");
			return;
		}

		// 등록해뒀던 사용자 정의 구조체를 콜백 함수의 인자로 받음.
		var recvData = (AsyncRecvData)asyncResult.AsyncState;

		try
		{
			// 비동기 IO를 이제 끝내고 받은 바이트 수를 추가해준다.
			recvData.RecvSize += recvData.Socket.EndReceive(asyncResult);
			// 읽었던 위치를 처음으로 돌려준다.
			recvData.ReadPos = 0;
		}
		catch (SocketException e)
		{
			HandleException(e);
			return;
		}

		// 받은 데이터로부터 패킷을 만든다.
		while (true)
		{
			// 헤더 사이즈보다 적은 데이터가 있다면 더 이상 패킷을 만들지 않음.
			if (recvData.RecvSize < NetworkDefinition.PacketHeaderSize)
			{
				break;
			}

			// 패킷 헤더 조제.
			var header = new PacketHeader()
			{
				PacketId = BitConverter.ToInt32(recvData.Buffer, 0),
				BodySize = BitConverter.ToInt32(recvData.Buffer, 4)
			};

			Debug.LogFormat("Recv packet id {0}, size {1}", header.PacketId, header.BodySize);

			var byteData = new byte[header.BodySize];
			Buffer.BlockCopy(recvData.Buffer, 8, byteData, 8, header.BodySize);

			var receivedPacket = new Packet
			{
				PacketId = header.PacketId,
				BodySize = header.BodySize,
				Data = byteData
			};

			// 받은 패킷을 큐로 넣어준다.
			lock (packetQueue)
			{
				packetQueue.Enqueue(receivedPacket);
			}

			// 조제한 데이터 만큼 갱신해준다.
			recvData.ReadPos += NetworkDefinition.PacketHeaderSize + header.BodySize;
			recvData.RecvSize -= NetworkDefinition.PacketHeaderSize + header.BodySize;
		}

		// 다시 비동기 Recv를 걸어준다.
		recvData.Socket.BeginReceive(
			recvData.Buffer,
			recvData.RecvSize,                           // 받아 놓은 길이에서부터 recv 시작.
			recvData.Buffer.Length - recvData.RecvSize, // 받아 놓은 길이만큼 버퍼길이가 줄어든 상태.
			SocketFlags.None,
			recvCallBack,
			recvData);
	}

	// Send IO 작업이 끝났을 때 호출 될 콜백 메소드.
	private void SendCallBack(IAsyncResult asyncResult)
	{
		if (IsConnected == false)
		{
			return;
		}

		var sendData = (AsyncSendData)asyncResult;

		var sendedSize = 0;

		try
		{
			// 비동기 Send 요청을 끝내준다.
			sendedSize = sendData.Socket.EndSend(asyncResult);
		}
		catch (SocketException e)
		{
			HandleException(e);
		}

		// 만약 요청한 사이즈보다 보낸 데이터가 작다면
		if (sendedSize < sendData.SendSize)
		{
			// 다시 비동기 Send를 요청.
			socket.BeginSend(
				sendData.Buffer,
				sendedSize,
				sendData.SendSize - sendedSize,
				SocketFlags.Truncated,              // 메시지가 너무 커서 잘렸을 경우의 플래그.
				sendCallBack,
				sendData);

			Debug.LogAssertion("Sended size is smaller than requested size");
		}

		Debug.Log("Packet send end!");
	}

	// SocketException을 처리하는 메소드.
	private static void HandleException(ExternalException e)
	{
		var errorCode = (SocketError)e.ErrorCode;

		Debug.LogAssertion(e.Message);

		if (errorCode == SocketError.ConnectionAborted ||
			errorCode == SocketError.Disconnecting ||
			errorCode == SocketError.HostDown ||
			errorCode == SocketError.Shutdown ||
			errorCode == SocketError.SocketError ||
			errorCode == SocketError.ConnectionReset)
		{
			// TODO :: Close Connect 알림을 서버에 던짐.
		}
	}
}

// 비동기 수신에 사용할 구조체.
internal class AsyncRecvData
{
    public byte[] Buffer;
    public Socket Socket;
    public int RecvSize;
    public int ReadPos;

    public AsyncRecvData(int bufferSize, Socket socket)
    {
        RecvSize = 0;
        ReadPos = 0;
        Buffer = new byte[bufferSize];
        Socket = socket;
    }
}

// 비동기 발신에 사용할 구조체.
internal class AsyncSendData
{
    public byte[] Buffer;
    public Socket Socket;
    public int SendSize;

    public AsyncSendData(Socket socket)
    {
        Socket = socket;
        SendSize = 0;
    }
}