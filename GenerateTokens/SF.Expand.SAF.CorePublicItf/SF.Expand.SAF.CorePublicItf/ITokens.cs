using System;
using System.ComponentModel;
namespace SF.Expand.SAF.CorePublicItf
{
	public interface ITokens
	{
		[Description("Create/Associate one token with one application code, namely userID")]
		OperationResult Create(string tokenVendorID, DateTime expirationDate, string supplierSerialNumber, string creationLotID, string pin, out TokenInfoCore tokenInfoCore);
		[Description("Undo creation function")]
		OperationResult UndoCreate(string tokenInternalID);
		[Description("Block select token from autenticate")]
		OperationResult InhibitedUse(string tokenInternalID);
		[Description("Allow selected token to start autenticate")]
		OperationResult AllowedUse(string tokenInternalID);
		[Description("Cancel use of the selected token")]
		OperationResult Cancel(string tokenInternalID);
		[Description("Verify if token is ready to autenticate")]
		OperationResult CheckStatus(string tokenInternalID, out TokenStatus tokenStatus);
	}
}
