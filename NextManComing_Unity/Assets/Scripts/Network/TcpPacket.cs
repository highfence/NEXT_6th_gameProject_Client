using MessagePack;
using System;
using System.Collections.Generic;

namespace TcpPacket
{
	internal enum PacketId
	{
		// Manage Server 관련 패킷은 100번대 사용.

		ServerListReq = 110,
		ServerListRes = 111,
		
		// 게임 서버 관련 패킷은 200번대 사용.

		ServerConnectReq = 201,
		ServerConnectRes = 202,

	}

	[MessagePackObject]
	public class PacketHeader
	{
		[Key(0)]
		public int PacketId;
		[Key(1)]
		public int BodySize;
	}

	public class Packet
	{
		public int PacketId;
		public int BodySize;
		public byte[] Data;
	}

	[MessagePackObject]
	public class ServerListReq
	{
		public string Id;
		public Int64 Token;
	}

	[MessagePackObject]
	public class ServerListRes
	{
		public int Result;
		public int ServerCount;
		public List<string> ServerList;
		public List<int> ServerCountList;
	}

	[MessagePackObject]
	public class ServerConnectReq
	{
		[Key(0)]
		public string Id;
		[Key(1)]
		public Int64 Token;
	}

	[MessagePackObject]
	public class ServerConnectRes
	{
		[Key(0)]
		public int Result;
	}
}