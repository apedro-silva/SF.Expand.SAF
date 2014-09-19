using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public enum OperationResult
	{
		Success,
		Error = -1,
		PreValidationRulesFail = 80,
		PostValidationRulesFail,
		TokenVendorSeedNotAvaliable,
		WrongStatusForRequestedOperation
	}
}
