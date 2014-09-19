using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public enum TokenDeviceType
	{
		HardwareEmbedded,
		SoftwareServerSide,
		SoftwareBothSidesSync,
		TransactionAuthenticationNumber
	}
}
