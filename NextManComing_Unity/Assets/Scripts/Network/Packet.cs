
namespace Packet
{
	enum PacketId
	{

	}

	public class PacketHeader
	{
		public int PacketId;
		public int BodySize;
		public string Data;
	}
}