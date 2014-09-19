using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public class TokenValidationsCount
	{
		public int TokenType;
		public int FailsCount;
		public int SuccessCount;
		public static TokenValidationsCount loadTokensValidationsCount(int TokenType, int FailsCount, int SuccessCount)
		{
			return new TokenValidationsCount
			{
				TokenType = TokenType,
				FailsCount = FailsCount,
				SuccessCount = SuccessCount
			};
		}
	}
}
