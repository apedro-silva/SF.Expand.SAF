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
			TokenInfo _newTkInfoBus = new TokenInfo();
			_newTkInfoBus.Status = status;
			_newTkInfoBus.TypeDescription = ((typeDescription == null) ? null : typeDescription.Trim());
			_newTkInfoBus.ApplicationUser = ((applicationUser == null) ? null : applicationUser.Trim());
			_newTkInfoBus.PhoneNumberUser = ((phoneNumberUser == null) ? null : phoneNumberUser.Trim());
			_newTkInfoBus.EmailAddressUser = ((emailAddressUser == null) ? null : emailAddressUser.Trim());
			_newTkInfoBus.RegisteredTimeStamp = registeredTimeStamp;
			_newTkInfoBus.LastStatusChangedTimeStamp = lastStatusTimeStamp;
			if (tokenInfoCore.InternalID == 0)
			{
				_newTkInfoBus.tokenInfoCore = TokenInfoCore.loadTokenInfoCore(typeID, internalID, null, null, internalSerialNumber, registeredTimeStamp, null, status);
			}
			else
			{
				_newTkInfoBus.tokenInfoCore = tokenInfoCore;
			}
			return _newTkInfoBus;
		}
	}
}
