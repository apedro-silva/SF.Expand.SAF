using SF.Expand.LOG;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.CryptoEngine;
using SF.Expand.SAF.Defs;
using System;
using System.Reflection;
namespace SF.Expand.SAF.Core
{
	public static class HOTPCryptoData
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.HOTPCryptoData.softfinanca.com/";
		public static OperationResult Generate(string masterKey, string pin, TokenTypeBaseParams tokenTypeBaseParams, out byte[] tkseed, out byte[] tkserial, out long tkmovFactor)
		{
			OperationResult result;
			try
			{
				if (tokenTypeBaseParams.SeedType != TokenSeedType.Dynamic)
				{
					throw new Exception("Invalid token info!");
				}
				tkserial = HOTPCipherInitialize.createSerialNumber((pin == null || pin.Length < 1) ? HOTPCipherInitialize.Generate4DigitsPin() : pin);
				byte[] buffOPSeed = HOTPCipherInitialize.createSeed((masterKey == null || masterKey.Length < 1) ? new byte[0] : BaseFunctions.convertStringToByteArray(masterKey));
				tkseed = HOTPCipher.encryptData(buffOPSeed, HOTPCipherInitialize.createCryptKey(tkserial, (masterKey == null || masterKey.Length < 1) ? new byte[0] : BaseFunctions.convertStringToByteArray(masterKey)));
				if (tokenTypeBaseParams.MovingFactorType == TokenMovingFactorType.EventBase)
				{
					tkmovFactor = HOTPCipherInitialize.createSequenceNumber();
				}
				else
				{
					tkmovFactor = -1L;
				}
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				tkseed = null;
				tkserial = null;
				tkmovFactor = -1L;
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.HOTPCryptoData.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			finally
			{
			}
			return result;
		}
	}
}
