using System.Runtime.InteropServices;
using MessagePack;
using System;

namespace PacketInfo
{
	enum PacketId
	{
		ServerConnectReq = 101,
		ServerConnectRes = 102
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
	public class Packet
	{
		[Key(0)]
		public int PacketId;
		[Key(1)]
		public int BodySize;
		[Key(2)]
		public byte[] Data;
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