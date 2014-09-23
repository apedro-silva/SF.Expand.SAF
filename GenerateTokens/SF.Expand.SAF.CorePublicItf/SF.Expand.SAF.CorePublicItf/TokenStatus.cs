using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public enum TokenStatus
	{
		Undefined = 99,
		ReadyToDeploy = 98,
		Enabled = 1,
		Disabled = 0,
		Canceled = 9
	}
}
