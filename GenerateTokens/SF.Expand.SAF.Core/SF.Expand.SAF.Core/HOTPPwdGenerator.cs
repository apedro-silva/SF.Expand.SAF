using SF.Expand.SAF.CryptoEngine;
using SF.Expand.SAF.Defs;
using System;
namespace SF.Expand.SAF.Core
{
	public static class HOTPPwdGenerator
	{
		public static string generate(TokenCryptoData tokenCryptoData, string masterKey, byte[] entropy)
		{
			return HOTPPwdGenerator.generate(tokenCryptoData, tokenCryptoData.GetTokenSeed(masterKey), entropy);
		}
		public static string generate(TokenCryptoData tokenCryptoData, byte[] tokenSeed, byte[] entropy)
		{
			int num = (entropy != null) ? (entropy.Length + 8) : 8;
			long num2 = tokenCryptoData.CryptoData.MovingFactor;
			byte[] array = new byte[num];
			byte[] array2 = new byte[20];
			int[] array3 = new int[]
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
			for (int i = num - 1; i >= 0; i--)
			{
				if (num - i <= 8)
				{
					array[i] = (byte)(num2 & 255L);
					num2 >>= 8;
				}
				else
				{
					array[i] = entropy[i];
				}
			}
			array2 = CryptoEngineHMACSHA1.process(tokenSeed, array, SHA1HashFunction.HashSize);
			int num3 = (int)(array2[array2.Length - 1] & 15);
			if (0 <= tokenCryptoData.TokenBaseParams.OTPOffSet && tokenCryptoData.TokenBaseParams.OTPOffSet < array2.Length - 4)
			{
				num3 = tokenCryptoData.TokenBaseParams.OTPOffSet;
			}
			int num4 = (int)(array2[num3] & 127) << 24 | (int)(array2[num3 + 1] & 255) << 16 | (int)(array2[num3 + 2] & 255) << 8 | (int)(array2[num3 + 3] & 255);
			int value = num4 % array3[tokenCryptoData.TokenBaseParams.OTPTotalDigits];
			return Convert.ToString(value, 10).PadLeft(tokenCryptoData.TokenBaseParams.OTPTotalDigits, '0');
		}
	}
}
