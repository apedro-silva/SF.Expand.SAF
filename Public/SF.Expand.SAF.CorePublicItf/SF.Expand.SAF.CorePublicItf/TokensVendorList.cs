using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public class TokensVendorList
	{
		public string TokenVendorID;
		public string TokenMovingFactorType;
		public string TokenVendorDescription;
		public bool TokenVendorStatus;
		public static TokensVendorList LoadTokensVendorList(string tokenVendorID, string tokenVendorDescription, string tokenMovingFactorType, bool tokenVendorStatus)
		{
			return new TokensVendorList
			{
				TokenVendorID = tokenVendorID,
				TokenMovingFactorType = tokenMovingFactorType,
				TokenVendorDescription = tokenVendorDescription,
				TokenVendorStatus = tokenVendorStatus
			};
		}
	}
}
