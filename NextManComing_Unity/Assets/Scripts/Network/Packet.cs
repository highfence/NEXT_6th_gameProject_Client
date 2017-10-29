using System.Runtime.InteropServices;
using MessagePack;
using System;

namespace Packet
{
	enum PacketId
	{

	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
	public class PacketHeader
	{
		public int PacketId;
		public int BodySize;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
		public string Data;
	}

	public class ServerConnectReq
	{
		public string Id;
		public Int64 Token;
	}
}