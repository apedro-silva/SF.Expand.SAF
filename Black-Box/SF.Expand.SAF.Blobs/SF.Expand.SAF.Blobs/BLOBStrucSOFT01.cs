using SF.Expand.LOG;
using SF.Expand.SAF.Defs;
using System;
namespace SF.Expand.SAF.Blobs
{
	internal class BLOBStrucSOFT01 : IBLOBData
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.BLOBStrucSOFT01.softfinanca.com/";
		public bool Export(string pin, string deviceType, string masterKey, TokenCryptoData tokenCryptoData, out string tokenBlob)
		{
			SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
			{
				"http://sfexpand.SAFCore.BLOBStrucSOFT01.softfinanca.com/",
				new NotImplementedException().ToString()
			});
			throw new NotImplementedException();
		}
		public bool Import(string tokenBlob, string blobCryptoPasswd, out TokenCryptoData tokenCryptoData)
		{
			SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
			{
				"http://sfexpand.SAFCore.BLOBStrucSOFT01.softfinanca.com/",
				new NotImplementedException().ToString()
			});
			throw new NotImplementedException();
		}
	}
}
