
using System.Collections.Generic;

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
		public string ManageServerAddr;
		public int ManageServerPort;
	}

	public struct LogoutReq
	{
		public string UserId;
		public string UserPw;
		public long Token;
	}
}