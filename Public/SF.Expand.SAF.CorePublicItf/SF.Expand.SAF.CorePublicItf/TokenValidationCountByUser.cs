using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public class TokenValidationCountByUser
	{
		public string ApplicationUser;
		public string TokenType;
		public int TotalCount;
		public int SucessCount;
		public int FailCount;
		public static TokenValidationCountByUser loadValidationCountByUser(string ApplicationUser, string TokenType, int TotalCount, int SucessCount, int FailCount)
		{
			return new TokenValidationCountByUser
			{
				ApplicationUser = ApplicationUser,
				TokenType = TokenType,
				TotalCount = TotalCount,
				SucessCount = SucessCount,
				FailCount = FailCount
			};
		}
	}
}
