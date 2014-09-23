using System;
using System.Web.Services.Protocols;
namespace SF.Expand.SAF.CorePublicItf
{
	public class AuthHeader : SoapHeader
	{
		public enum CryptoAlgorithmEnum
		{
			NONE,
			DES,
			TRIPLEDES,
			RIJNDAEL
		}
		public string AuthKey;
		public AuthHeader.CryptoAlgorithmEnum CryptoAlgorithm;
	}
}
