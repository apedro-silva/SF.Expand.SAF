using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public enum AutenticationStatus
	{
		Success,
		AutenticationProcessFail = -1,
		SuccessButSynchronized = 200,
		ErrorCheckTokenStatus,
		TokenOrPasswordInvalid,
		InvalidDataOnPasswordValidation,
		TokenNotFoundOrCanceled,
		TokenOrPasswordInvalidMultiple,
		PreValidationRulesFail,
		PostValidationRulesFail
	}
}
