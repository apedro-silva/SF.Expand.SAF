using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public enum OperationResult
	{
		Success,
		Error = -1,
		TokenVendorSeedNotAvaliable = 99,
		WrongStatusForRequestedOperation = 9
	}
}
