﻿using System.Runtime.InteropServices;

static internal class NetworkDefinition
{
	static public int BufferSize = 8192;
	static public System.Text.Encoding NetworkEncoding = System.Text.Encoding.UTF8;
	static public int PacketHeaderSize = Marshal.SizeOf(typeof(PacketHeader));
	static public int IntSize = sizeof(int);
}