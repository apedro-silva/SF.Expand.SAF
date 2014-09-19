using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public class TokenTotals
	{
		public string TokenType;
		public int CreatedTokens;
		public int EnabledTokens;
		public int CanceledTokens;
		public int InstalledTokens;
		public int UndeployedTokens;
		public int ErrorTokens;
		public static TokenTotals loadTokenTotals(string TokenType, int CreatedTokens, int EnabledTokens, int CanceledTokens, int InstalledTokens, int UndeployedTokens, int ErrorTokens)
		{
			return new TokenTotals
			{
				TokenType = TokenType,
				CreatedTokens = CreatedTokens,
				EnabledTokens = EnabledTokens,
				CanceledTokens = CanceledTokens,
				InstalledTokens = InstalledTokens,
				UndeployedTokens = UndeployedTokens,
				ErrorTokens = ErrorTokens
			};
		}
	}
}
