using MessagePack;

enum PacketId
{

}

class PacketHeader
{

}

[MessagePackObject]
public class Packet
{
	[Key(0)]
	public int PacketId;
	[Key(1)]
	public int BodySize;
	[Key(2)]
	public string Data;
}
