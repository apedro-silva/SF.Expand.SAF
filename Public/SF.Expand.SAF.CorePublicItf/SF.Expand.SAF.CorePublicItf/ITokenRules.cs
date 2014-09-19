using System;
using System.ComponentModel;
namespace SF.Expand.SAF.CorePublicItf
{
	public interface ITokenRules
	{
		[Description("Rules to execute before token creation!")]
		OperationResult BeforeCreate(string applicationUser, string applicationUseruserPhone, string applicationEmail, string tokenVendorID, string expirationDate, string supplierSerialNumber, string creationLotID, string pin, string baseNotifyMessage);
		[Description("Rules to execute after token creation!")]
		OperationResult AfterCreate(string applicationUser, string applicationUseruserPhone, string applicationEmail, string tokenVendorID, string expirationDate, string supplierSerialNumber, string creationLotID, string pin, string baseNotifyMessage, int tokenInternalID, long businessEventID, TokenStatus tokenStatus);
		[Description("Rules to execute before token cancel!")]
		OperationResult BeforeCancel(string applicationUser, string tokenID, string baseNotifyMessage, TokenStatus tokenCurrentStatus);
		[Description("Rules to execute after token cancel!")]
		OperationResult AfterCancel(string applicationUser, string tokenID, string baseNotifyMessage);
		[Description("Rules to execute before token disable!")]
		OperationResult BeforeDisable(string applicationUser, string tokenID, string baseNotifyMessage, TokenStatus tokenCurrentStatus);
		[Description("Rules to execute after token disable!")]
		OperationResult AfterDisable(string applicationUser, string tokenID, string baseNotifyMessage);
		[Description("Rules to execute before token enable!")]
		OperationResult BeforeEnable(string applicationUser, string tokenID, string baseNotifyMessage, TokenStatus tokenCurrentStatus);
		[Description("Rules to execute after token enable!")]
		OperationResult AfterEnable(string applicationUser, string tokenID, string baseNotifyMessage);
		[Description("Rules to execute before token StartServerAuthentication!")]
		OperationResult BeforeStartServerAuthentication(string applicationUser, string tokenID, string baseNotifyMessage, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType);
		[Description("Rules to execute After token StartServerAuthentication!")]
		OperationResult AfterStartServerAuthentication(string applicationUser, string tokenID, string baseNotifyMessage, string newPassword, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType);
		[Description("Rules to execute before token Synchronize!")]
		OperationResult BeforeSynchronize(string applicationUser, string tokenID, string baseNotifyMessage, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType);
		[Description("Rules to execute after token Synchronize!")]
		OperationResult AfterSynchronize(string applicationUser, string tokenID, string baseNotifyMessage, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType);
		[Description("Rules to execute Before token Autenticate !")]
		AutenticationStatus BeforeAutenticate(string applicationUser, string tokenID, string baseNotifyMessage, bool onLoopValidation, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType);
		[Description("Rules to execute after token Autenticate!")]
		AutenticationStatus AfterAutenticate(string applicationUser, string tokenID, string baseNotifyMessage, bool onLoopValidation, string newChallenge, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType);
		[Description("Rules to execute before token ChallengeRequest!")]
		OperationResult BeforeChallengeRequest(string applicationUser, string tokenID, string baseNotifyMessage, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType);
		[Description("Rules to execute after token ChallengeRequest!")]
		OperationResult AfterChallengeRequest(string applicationUser, string tokenID, string baseNotifyMessage, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType);
	}
}
