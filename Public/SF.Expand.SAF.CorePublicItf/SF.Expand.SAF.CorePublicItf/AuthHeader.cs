using System;
using System.Web.Services.Protocols;
namespace SF.Expand.SAF.CorePublicItf
{
	public class AuthHeader : SoapHeader
	{
		public enum CryptoAlgorithmEnum
		{
			NONE,
			TRIPLEDES
		}
		public string AuthKey;
		public AuthHeader.CryptoAlgorithmEnum CryptoAlgorithm;
	}
}
