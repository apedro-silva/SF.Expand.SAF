using System;
using System.ComponentModel;
namespace SF.Expand.SAF.CorePublicItf
{
	public interface ITokenValidator
	{
		[Description("Authenticate sent password")]
		AutenticationStatus Autenticate(string tokenInternalID, string password, string dataEntropy, out string Challenge);
		[Description("Challenge/Response request")]
		OperationResult ChallengeRequest(string tokenInternalID, string dataEntropy, out string Challenge);
		[Description("Start server side authentication process")]
		OperationResult StartServerAuthentication(string tokenInternalID, string dataEntropy, out string newPwd);
		[Description("Synchronize client token with the server")]
		OperationResult Synchronize(string tokenInternalID, string firstPwd, string secondPwd);
		[Description("Reset active Challenge Request for this token")]
		OperationResult ResetChallengeRequest(string tokenInternalID);
		[Description("Reset current moving factor value for this token")]
		OperationResult ResetMovingFactor(string tokenInternalID, long movingFactorValue);
	}
}
