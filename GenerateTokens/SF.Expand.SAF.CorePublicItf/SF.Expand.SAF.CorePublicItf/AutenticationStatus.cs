using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public enum AutenticationStatus
	{
		Success,
		AutenticationProcessFail = -1,
		SuccessButSynchronized = 100,
		ErrorCheckTokenStatus,
		TokenOrPasswordInvalid,
		InvalidDataOnPasswordValidation,
		TokenNotFoundOrCanceled = 109,
		TokenOrPasswordInvalidMultiple
	}
}
