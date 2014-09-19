using SF.Expand.SAF.Defs;
using System;
using System.ComponentModel;
namespace SF.Expand.SAF.Blobs
{
	public interface IBLOBData
	{
		[Description("Export cryptografic data in blob format!")]
		bool Export(string pin, string deviceType, string masterKey, TokenCryptoData tokenCryptoData, out string tokenBlob);
		[Description("Translate blob struct into internal/suported info!")]
		bool Import(string tokenBlob, string blobCryptoPasswd, out TokenCryptoData tokenCryptoData);
	}
}
