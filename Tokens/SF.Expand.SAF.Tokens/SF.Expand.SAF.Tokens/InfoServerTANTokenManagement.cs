using SF.Expand.SAF.Core;
using SF.Expand.SAF.CorePublicItf;
using System;
namespace SF.Expand.SAF.Tokens
{
	public class InfoServerTANTokenManagement : ITokens
	{
		public OperationResult Create(string tokenVendorID, DateTime expirationDate, string supplierSerialNumber, string creationLotID, string pin, out TokenInfoCore tokenInfoCore)
		{
			return new TokensDAO().newTokenFromGivenSupplierSerialNumber(tokenVendorID, supplierSerialNumber, out tokenInfoCore);
		}
		public OperationResult UndoCreate(string tokenInternalID)
		{
			return new TokensDAO().undoUpdateForNewToken(tokenInternalID);
		}
		public OperationResult InhibitedUse(string tokenInternalID)
		{
			return new TokensDAO().updateTokenStatus(tokenInternalID, TokenStatus.Disabled);
		}
		public OperationResult AllowedUse(string tokenInternalID)
		{
			return new TokensDAO().updateTokenStatus(tokenInternalID, TokenStatus.Enabled);
		}
		public OperationResult Cancel(string tokenInternalID)
		{
			return new TokensDAO().updateTokenStatus(tokenInternalID, TokenStatus.Canceled);
		}
		public OperationResult CheckStatus(string tokenInternalID, out TokenStatus tokenStatus)
		{
			return new TokensDAO().tokenStatus(tokenInternalID, out tokenStatus);
		}
	}
}
