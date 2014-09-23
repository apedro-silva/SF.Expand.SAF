using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public class TokensUser
	{
		private string ApplicationID;
		private TokenInfo[] TokensInfo;
		public static TokensUser LoadTokensUser(string tokenApplicationID, TokenInfo[] tokensInfo)
		{
			return new TokensUser
			{
				ApplicationID = tokenApplicationID,
				TokensInfo = tokensInfo
			};
		}
	}
}
