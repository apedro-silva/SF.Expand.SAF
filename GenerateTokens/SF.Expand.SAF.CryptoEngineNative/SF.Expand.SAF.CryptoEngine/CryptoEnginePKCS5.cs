using System;
namespace SF.Expand.SAF.CryptoEngine
{
	public static class CryptoEnginePKCS5
	{
		public static byte[] pbkdf2(byte[] Password, byte[] Salt, int Iterator, int Length)
		{
			int hashSize = SHA1HashFunction.HashSize;
			byte[] data = new byte[hashSize];
			byte[] sourceArray = new byte[hashSize];
			int num7 = Salt.Length + 4;
			byte[] destinationArray = new byte[Length];
			if (Length > 29 * hashSize)
			{
				throw new Exception("Derived key too long.");
			}
			if (Iterator <= 0)
			{
				throw new Exception("Invalid Iterator specified.");
			}
			if (Length <= 0)
			{
				throw new Exception("Invalid Length specified.");
			}
			int num8 = (Length - 1) / hashSize + 1;
			int num9 = Length - (num8 - 1) * hashSize;
			byte[] buffer3 = new byte[num7];
			Array.Copy(Salt, 0, buffer3, 0, Salt.Length);
			for (int i = 1; i <= num8; i++)
			{
				sourceArray = new byte[hashSize];
				for (int j = 1; j <= Iterator; j++)
				{
					if (j == 1)
					{
						buffer3[Salt.Length] = (byte)(i >> 24 & 255);
						buffer3[Salt.Length + 1] = (byte)(i >> 16 & 255);
						buffer3[Salt.Length + 2] = (byte)(i >> 8 & 255);
						buffer3[Salt.Length + 3] = (byte)(i & 255);
						data = CryptoEngineHMACSHA1.process(Password, buffer3, hashSize);
					}
					else
					{
						data = CryptoEngineHMACSHA1.process(Password, data, hashSize);
					}
					for (int k = 0; k < hashSize; k++)
					{
						sourceArray[k] ^= data[k];
					}
				}
				Array.Copy(sourceArray, 0, destinationArray, (i - 1) * hashSize, (i == num8) ? num9 : hashSize);
			}
			return destinationArray;
		}
	}
}
