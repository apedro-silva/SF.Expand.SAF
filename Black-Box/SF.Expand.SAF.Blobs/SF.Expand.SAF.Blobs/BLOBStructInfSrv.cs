using SF.Expand.LOG;
using SF.Expand.SAF.Core;
using SF.Expand.SAF.CryptoEngine;
using SF.Expand.SAF.Defs;
using System;
using System.Reflection;
namespace SF.Expand.SAF.Blobs
{
	public class BLOBStructInfSrv : IBLOBData
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.BLOBStructInfSrv.softfinanca.com/";
		private const int BLOB_COMPLETE_SIZE = 114;
		private const int BLOB_CRIPTOBLOCK_SIZE = 48;
		private const int OTP_OFFSET_SIZE = 1;
		private const int OTP_TOTALDIGITS_SIZE = 1;
		private const int OTP_MOVINGFACTORTYPE_SIZE = 1;
		private const int OTP_MOVINGFACTORDRIF_SIZE = 1;
		private const int OTP_MOVINGFACTOR_SIZE = 8;
		private const int OTP_SERIALNUMBER_SIZE = 32;
		private const int OTP_INTERNALKEY_SIZE = 32;
		private const int TOKEN_DEVICETYPE_SIZE = 2;
		private const int TOKEN_DEVICEPIN_SIZE = 32;
		public bool Export(string pin, string deviceType, string masterKey, TokenCryptoData tokenCryptoData, out string tokenBlob)
		{
			tokenBlob = null;
			bool result;
			try
			{
				if (pin == null || pin.Length < 4 || deviceType == null || deviceType.Length != 2)
				{
					result = false;
				}
				else
				{
					byte[] blob = new byte[114];
					byte[] cryptoBlock = new byte[48];
					byte[] _oTPOffSet = new byte[]
					{
						(byte)tokenCryptoData.TokenBaseParams.OTPOffSet
					};
					byte[] _oTPTotalDigits = new byte[]
					{
						(byte)tokenCryptoData.TokenBaseParams.OTPTotalDigits
					};
					byte[] _movingFactorType = new byte[]
					{
						(byte)tokenCryptoData.TokenBaseParams.MovingFactorType
					};
					byte[] _hOTPMovingFactorDrift = new byte[]
					{
						(byte)tokenCryptoData.TokenBaseParams.HOTPMovingFactorDrift
					};
					byte[] _dvType = new byte[]
					{
						(char.Parse(deviceType.Substring(0, 1)) <= '9' && char.Parse(deviceType.Substring(0, 1)) >= '0') ? byte.Parse(deviceType.Substring(0, 1)) : ((byte)char.Parse(deviceType.Substring(0, 1))),
						(char.Parse(deviceType.Substring(1, 1)) <= '9' && char.Parse(deviceType.Substring(1, 1)) >= '0') ? byte.Parse(deviceType.Substring(1, 1)) : ((byte)char.Parse(deviceType.Substring(1, 1)))
					};
					byte[] _pin = HashBaseFunction.createBinaryHash(BaseFunctions.convertStringToByteArray(pin));
					byte[] _movingFactor = BitConverter.GetBytes(tokenCryptoData.CryptoData.MovingFactor);
					byte[] _serialNumber = BaseFunctions.HexDecoder(tokenCryptoData.CryptoData.InternalSerialNumber);
					byte[] _internalKey = tokenCryptoData.GetTokenSeed(masterKey);
					Buffer.BlockCopy(_internalKey, 0, cryptoBlock, 0, 32);
					Buffer.BlockCopy(_movingFactor, 0, cryptoBlock, 32, 8);
					Buffer.BlockCopy(_oTPTotalDigits, 0, cryptoBlock, 40, 1);
					Buffer.BlockCopy(_oTPOffSet, 0, cryptoBlock, 41, 1);
					Buffer.BlockCopy(_movingFactorType, 0, cryptoBlock, 42, 1);
					Buffer.BlockCopy(_hOTPMovingFactorDrift, 0, cryptoBlock, 43, 1);
					Buffer.BlockCopy(_dvType, 0, blob, 0, 2);
					Buffer.BlockCopy(_pin, 0, blob, 2, 32);
					Buffer.BlockCopy(_serialNumber, 0, blob, 34, 32);
					Buffer.BlockCopy(HOTPCipher.encrypt(cryptoBlock, HOTPCipherInitialize.createCryptKey(_serialNumber, pin)), 0, blob, 66, 48);
					tokenBlob = BaseFunctions.HexEncoder(blob);
					result = true;
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.BLOBStructInfSrv.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				tokenBlob = null;
				result = false;
			}
			return result;
		}
		public bool Import(string tokenBlob, string blobCryptoPasswd, out TokenCryptoData tokenCryptoData)
		{
			throw new NotImplementedException();
		}
	}
}
