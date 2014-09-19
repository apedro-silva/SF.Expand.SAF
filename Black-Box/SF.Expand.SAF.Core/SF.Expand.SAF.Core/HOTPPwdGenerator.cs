using SF.Expand.LOG;
using SF.Expand.SAF.CryptoEngine;
using SF.Expand.SAF.Defs;
using System;
using System.Reflection;
namespace SF.Expand.SAF.Core
{
	public static class HOTPPwdGenerator
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.HOTPPwdGenerator.softfinanca.com/";
		public static string generate(TokenCryptoData tokenCryptoData, string masterKey, byte[] entropy)
		{
			return HOTPPwdGenerator.generate(tokenCryptoData, tokenCryptoData.GetTokenSeed(masterKey), entropy);
		}
		public static string generate(TokenCryptoData tokenCryptoData, byte[] tokenSeed, byte[] entropy)
		{
			int num = (entropy != null) ? (entropy.Length + 8) : 8;
			long _movingFactor = tokenCryptoData.CryptoData.MovingFactor;
			int[] DIGITS_POWER = new int[]
			{
				1,
				10,
				100,
				1000,
				10000,
				100000,
				1000000,
				10000000,
				100000000
			};
			byte[] buffer = new byte[num];
			byte[] buffer2 = new byte[20];
			string result;
			try
			{
				for (int i = num - 1; i >= 0; i--)
				{
					if (num - i <= 8)
					{
						buffer[i] = (byte)(_movingFactor & 255L);
						_movingFactor >>= 8;
					}
					else
					{
						buffer[i] = entropy[i];
					}
				}
				buffer2 = CryptoEngineHMACSHA1.process(tokenSeed, buffer, SHA1HashFunction.HashSize);
				int index = (int)(buffer2[buffer2.Length - 1] & 15);
				if (0 <= tokenCryptoData.TokenBaseParams.OTPOffSet && tokenCryptoData.TokenBaseParams.OTPOffSet < buffer2.Length - 4)
				{
					index = tokenCryptoData.TokenBaseParams.OTPOffSet;
				}
				int num2 = (int)(buffer2[index] & 127) << 24 | (int)(buffer2[index + 1] & 255) << 16 | (int)(buffer2[index + 2] & 255) << 8 | (int)(buffer2[index + 3] & 255);
				int num3 = num2 % DIGITS_POWER[tokenCryptoData.TokenBaseParams.OTPTotalDigits];
				result = Convert.ToString(num3, 10).PadLeft(tokenCryptoData.TokenBaseParams.OTPTotalDigits, '0');
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.HOTPPwdGenerator.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = null;
			}
			finally
			{
			}
			return result;
		}
	}
}
