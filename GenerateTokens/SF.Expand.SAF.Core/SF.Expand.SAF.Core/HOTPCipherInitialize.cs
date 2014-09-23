using SF.Expand.SAF.CryptoEngine;
using System;
using System.Security.Cryptography;
using System.Text;
namespace SF.Expand.SAF.Core
{
	public static class HOTPCipherInitialize
	{
		public static byte[] derivateKey(byte[] Password, byte[] Salt, int counter, int keyLength)
		{
			return CryptoEnginePKCS5.pbkdf2(Password, Salt, counter, keyLength);
		}
		public static byte[] createCryptKey(byte[] serialNumber)
		{
			string text = BaseFunctions.HexEncoder(serialNumber);
			return HashBaseFunction.createBinaryHash(text.Substring(serialNumber.Length - 4) + text.Substring(0, 6));
		}
		public static byte[] createCryptKey(byte[] serialNumber, byte[] masterKey)
		{
			return CryptoEnginePKCS5.pbkdf2(masterKey, serialNumber, 2000, 32);
		}
		public static byte[] createCryptKey(byte[] serialNumber, string pin)
		{
			string text = BaseFunctions.HexEncoder(serialNumber);
			return HashBaseFunction.createBinaryHash(text.Substring(text.Length - 4, 4) + pin + text.Substring(0, 2));
		}
		public static string createActivationKey(byte pinCheckDigit)
		{
			byte[] array = BaseFunctions.secureRandom(14);
			array[0] = pinCheckDigit;
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text += (int)((array[i] % 10 < 0) ? (array[i] % 10 + 10) : (array[i] % 10));
			}
			string text2 = text.Substring(0, 7);
			string text3 = text.Substring(7, 7);
			string text4 = string.Concat(BaseFunctions.calcChecksum(long.Parse(text2), 7));
			string text5 = string.Concat(BaseFunctions.calcChecksum(long.Parse(text3), 7));
			return string.Concat(new string[]
			{
				text2,
				text4,
				"-",
				text3,
				text5
			});
		}
		public static byte[] createSeed(byte[] masterKey)
		{
			byte[] array = BaseFunctions.secureRandom(32);
			byte[] array2;
			if (masterKey == null)
			{
				array2 = new byte[array.Length];
				array.CopyTo(array2, 0);
			}
			else
			{
				array2 = new byte[masterKey.Length + array.Length];
				masterKey.CopyTo(array2, 0);
				array.CopyTo(array2, masterKey.Length);
			}
			return HashBaseFunction.createBinaryHash(array2);
		}
		public static byte[] createSeed(string pin, string activationKey)
		{
			return HOTPCipherInitialize.derivateKey(BaseFunctions.convertStringToByteArray(activationKey), BaseFunctions.convertStringToByteArray(pin), 10, 32);
		}
		public static long createSequenceNumber()
		{
			byte[] array = BaseFunctions.secureRandom(2);
			return (long)(array[0] * 10 + array[1]);
		}
		public static byte[] createSerialNumber()
		{
			return HOTPCipherInitialize.createSerialNumber("");
		}
		public static byte[] createSerialNumber(string pin)
		{
			return HashBaseFunction.createBinaryHash(DateTime.Now.ToString("yyyyMMddHHmmssfff") + pin);
		}
		public static string Generate4DigitsPin()
		{
			RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
			string text;
			do
			{
				byte[] array = new byte[2];
				rNGCryptoServiceProvider.GetNonZeroBytes(array);
				text = Math.Abs(BitConverter.ToInt16(array, 0)).ToString();
			}
			while (text.Length != 4 || text.StartsWith("00") || text.EndsWith("00"));
			return text;
		}
		public static string GeneratePassword(int size, bool lowerCase)
		{
			int tickCount = Environment.TickCount;
			new Random();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < size; i++)
			{
				char value = Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * new Random(tickCount++).NextDouble() + 65.0)));
				stringBuilder.Append(value);
			}
			if (!lowerCase)
			{
				return stringBuilder.ToString();
			}
			return stringBuilder.ToString().ToLower();
		}
		public static string GenerateNumericSerialNumber(int size)
		{
			Guid guid = Guid.NewGuid();
			int num = (size != 0) ? size : 32;
			string text = Math.Abs(BitConverter.ToInt32(guid.ToByteArray(), 0)).ToString().PadRight(num - 1, '0');
			return text + BaseFunctions.calcChecksum(long.Parse(text), num);
		}
		public static string GenerateComplexPassword(int size)
		{
			size = ((size > 16 || size < 6) ? 8 : size);
			byte[] array = new byte[size];
			StringBuilder stringBuilder = new StringBuilder(size);
			char[] array2 = "|!#$%&=?@*+abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
			new RNGCryptoServiceProvider().GetNonZeroBytes(array);
			byte[] array3 = array;
			for (int i = 0; i < array3.Length; i++)
			{
				byte b = array3[i];
				stringBuilder.Append(array2[(int)b % (array2.Length - 1)]);
			}
			return stringBuilder.ToString();
		}
	}
}
