using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public class TokenUserTotals
	{
		public int TotalUsers;
		public int TotalUsersWithCanceledTokens;
		public int TotalUsersWithSmsToken;
		public int TotalUsersWithMobileTokens;
		public int TotalUsersWithSmsMobileTokens;
		public int TotalUsersMobileActiveTokens;
		public int TotalUsersWithMobileUndeployedTokens;
		public int TotalUsersWithMobileErrorTokens;
		public static TokenUserTotals loadTokenUserTotals(int TotalUsers, int TotalUsersWithCanceledTokens, int TotalUsersWithSmsToken, int TotalUsersWithMobileTokens, int TotalUsersWithSmsMobileTokens, int TotalUsersMobileActiveTokens, int TotalUsersWithMobileUndeployedTokens, int TotalUsersWithMobileErrorTokens)
		{
			return new TokenUserTotals
			{
				TotalUsers = TotalUsers,
				TotalUsersWithCanceledTokens = TotalUsersWithCanceledTokens,
				TotalUsersWithSmsToken = TotalUsersWithSmsToken,
				TotalUsersWithMobileTokens = TotalUsersWithMobileTokens,
				TotalUsersWithSmsMobileTokens = TotalUsersWithSmsMobileTokens,
				TotalUsersMobileActiveTokens = TotalUsersMobileActiveTokens,
				TotalUsersWithMobileUndeployedTokens = TotalUsersWithMobileUndeployedTokens,
				TotalUsersWithMobileErrorTokens = TotalUsersWithMobileErrorTokens
			};
		}
	}
}
