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

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
	public class PacketHeader
	{
		public int PacketId;
		public int BodySize;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
	public class Packet
	{
		public int PacketId;
		public int BodySize;
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