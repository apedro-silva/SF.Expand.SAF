using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public class TokenInfo
	{
		public TokenStatus Status;
		public string TypeDescription;
		public string ApplicationUser;
		public string PhoneNumberUser;
		public string EmailAddressUser;
		public DateTime RegisteredTimeStamp;
		public DateTime LastStatusChangedTimeStamp;
		public TokenInfoCore tokenInfoCore;
		public static TokenInfo loadTokenInfo(TokenStatus status, byte typeID, int internalID, string typeDescription, string applicationUser, string phoneNumberUser, string emailAddressUser, DateTime registeredTimeStamp, string internalSerialNumber, DateTime lastStatusTimeStamp, TokenInfoCore tokenInfoCore)
		{
			TokenInfo tokenInfo = new TokenInfo();
			tokenInfo.Status = status;
			tokenInfo.TypeDescription = ((typeDescription == null) ? null : typeDescription.Trim());
			tokenInfo.ApplicationUser = ((applicationUser == null) ? null : applicationUser.Trim());
			tokenInfo.PhoneNumberUser = ((phoneNumberUser == null) ? null : phoneNumberUser.Trim());
			tokenInfo.EmailAddressUser = ((emailAddressUser == null) ? null : emailAddressUser.Trim());
			tokenInfo.RegisteredTimeStamp = registeredTimeStamp;
			tokenInfo.LastStatusChangedTimeStamp = lastStatusTimeStamp;
			if (tokenInfoCore.InternalID == 0)
			{
				tokenInfo.tokenInfoCore = TokenInfoCore.loadTokenInfoCore(typeID, internalID, null, null, internalSerialNumber, registeredTimeStamp, null, status);
			}
			else
			{
				tokenInfo.tokenInfoCore = tokenInfoCore;
			}
			return tokenInfo;
		}
	}
}
