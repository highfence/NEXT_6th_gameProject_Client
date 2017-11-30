
namespace HttpPacket
{
	public struct LoginReq
	{
		public string UserId;
		public string UserPw;
	}

	public struct LoginRes
	{
		public int Result;
		public long Token;
	}

	public struct LogoutReq
	{
		public string UserId;
		public string UserPw;
		public long Token;
	}
}