using SF.Expand.SAF.Core;
using SF.Expand.SAF.CorePublicItf;
using System;
namespace SF.Expand.SAF.Tokens
{
	public class GoogleAuthenticatorTokenManagement : ITokens
	{
		public OperationResult Create(string tokenVendorID, DateTime expirationDate, string supplierSerialNumber, string creationLotID, string pin, out TokenInfoCore tokenInfoCore)
		{
            return new TokensDAO().createToken(tokenVendorID, 0, expirationDate, "", supplierSerialNumber, supplierSerialNumber, "123444", null, out tokenInfoCore);
		}
		public OperationResult UndoCreate(string tokenInternalID)
		{
			return new TokensDAO().undoUpdateForNewToken(tokenInternalID);
		}
		public OperationResult Cancel(string tokenInternalID)
		{
			return new TokensDAO().updateTokenStatus(tokenInternalID, TokenStatus.Canceled);
		}
		public OperationResult InhibitedUse(string tokenInternalID)
		{
			return new TokensDAO().updateTokenStatus(tokenInternalID, TokenStatus.Disabled);
		}
		public OperationResult AllowedUse(string tokenInternalID)
		{
			return new TokensDAO().updateTokenStatus(tokenInternalID, TokenStatus.Enabled);
		}
		public OperationResult CheckStatus(string tokenInternalID, out TokenStatus tokenStatus)
		{
			return new TokensDAO().tokenStatus(tokenInternalID, out tokenStatus);
		}
	}
}
