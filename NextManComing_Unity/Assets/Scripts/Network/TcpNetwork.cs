﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using PacketInfo;

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
		packetQueue = new Queue<Packet.Packet>();

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
		int id;
		int bodySize;

		// 포인터 동작 수행.
		unsafe
		{
			#region PREPARE SENDING DATAS

			// 보낼 데이터 구조체를 Json 형태로 바꿔줌.
			string jsonData = JsonUtility.ToJson(data);

			id = (int)packetId;
			bodySize = jsonData.Length + 1;


			// Send 버퍼 생성.
			sendData._buffer = new byte[NetworkDefinition.PacketHeaderSize + bodySize + 1];
			sendData._sendSize = NetworkDefinition.PacketHeaderSize + bodySize;
			#endregion

			#region COPYING PACKET HEADER

			// 패킷 아이디 주소의 첫 번째 자리.
			byte* packetIdPos = (byte*)&id;

			// 포인터를 옮겨가며 버퍼에 기록.
			for (int i = 0; i < NetworkDefinition.IntSize; ++i)
			{
				sendData._buffer[i] = packetIdPos[i];
			}

			// 패킷 바디사이즈 주소의 첫 번째 자리.
			byte* bodySizePos = (byte*)&bodySize;

			// 아이디를 기록했던 버퍼 자리 뒤부터 기록.
			for (int i = 0; i < NetworkDefinition.IntSize; ++i)
			{
				sendData._buffer[NetworkDefinition.IntSize + i] = bodySizePos[i];
			}
			#endregion

			#region COPYING PACKET BODY

			// 패킷 바디 주소의 첫 번째 자리.
			char[] bodyPos = jsonData.ToCharArray();

			// 헤더를 기록했던 버퍼 자리 뒤부터 기록.
			for (int i = 0; i < bodySize - 1; ++i)
			{
				sendData._buffer[NetworkDefinition.PacketHeaderSize + i] = (byte)bodyPos[i];
			}

			// 뒤에 널 문자 추가.
			var nullChar = Convert.ToByte('\0');
			sendData._buffer[NetworkDefinition.PacketHeaderSize + bodySize] = nullChar;

			#endregion;
		}

		try
		{
			// 비동기 전송 시작. 파라미터는 BeginReceive와 대동소이하므로 생략. 
			socket.BeginSend(
				sendData._buffer,
				0,
				NetworkDefinition.PacketHeaderSize + bodySize,
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
			recvData._buffer,             // 버퍼
			0,                            // 받은 데이터를 저장할 zero-base postion
			recvData._buffer.Length,      // 버퍼 길이
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
			recvData._recvSize += recvData._socket.EndReceive(asyncResult);
			// 읽었던 위치를 처음으로 돌려준다.
			recvData._readPos = 0;
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
			if (recvData._recvSize < NetworkDefinition.PacketHeaderSize)
			{
				break;
			}

			// 패킷 헤더 조제.
			var header = new PacketHeader()
			{
				PacketId = BitConverter.ToInt32(recvData._buffer, 0),
				BodySize = BitConverter.ToInt32(recvData._buffer, 4)
			};

			Debug.LogFormat("Recv packet id {0}, size {1}", header.PacketId, header.BodySize);

			// 패킷 조제.
			var bodyJson = NetworkDefinition.NetworkEncoding.GetString(recvData._buffer, 8, header.BodySize);

			var receivedPacket = new Packet
			{
				PacketId = header.PacketId,
				BodySize = header.BodySize,
				Data = bodyJson
			};

			// 받은 패킷을 큐로 넣어준다.
			lock (packetQueue)
			{
				packetQueue.Enqueue(receivedPacket);
			}

			// 조제한 데이터 만큼 갱신해준다.
			recvData._readPos += NetworkDefinition.PacketHeaderSize + header.BodySize;
			recvData._recvSize -= NetworkDefinition.PacketHeaderSize + header.BodySize;
		}

		// 다시 비동기 Recv를 걸어준다.
		recvData._socket.BeginReceive(
			recvData._buffer,
			recvData._recvSize,                           // 받아 놓은 길이에서부터 recv 시작.
			recvData._buffer.Length - recvData._recvSize, // 받아 놓은 길이만큼 버퍼길이가 줄어든 상태.
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
			sendedSize = sendData._socket.EndSend(asyncResult);
		}
		catch (SocketException e)
		{
			HandleException(e);
		}

		// 만약 요청한 사이즈보다 보낸 데이터가 작다면
		if (sendedSize < sendData._sendSize)
		{
			// 다시 비동기 Send를 요청.
			socket.BeginSend(
				sendData._buffer,
				sendedSize,
				sendData._sendSize - sendedSize,
				SocketFlags.Truncated,              // 메시지가 너무 커서 잘렸을 경우의 플래그.
				sendCallBack,
				sendData);

			Debug.LogAssertion("Sended size is smaller than requested size");
		}

		Debug.Log("Packet send end!");
	}

	// SocketException을 처리하는 메소드.
	private void HandleException(SocketException e)
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
    public byte[] _buffer;
    public Socket _socket;
    public int _recvSize;
    public int _readPos;

    public AsyncRecvData(int bufferSize, Socket socket)
    {
        _recvSize = 0;
        _readPos = 0;
        _buffer = new byte[bufferSize];
        _socket = socket;
    }
}

// 비동기 발신에 사용할 구조체.
internal class AsyncSendData
{
    public byte[] _buffer;
    public Socket _socket;
    public int _sendSize;

    public AsyncSendData(Socket socket)
    {
        _socket = socket;
        _sendSize = 0;
    }
}