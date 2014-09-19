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
			string _str = BaseFunctions.HexEncoder(serialNumber);
			return HashBaseFunction.createBinaryHash(_str.Substring(serialNumber.Length - 4) + _str.Substring(0, 6));
		}
		public static byte[] createCryptKey(byte[] serialNumber, byte[] masterKey)
		{
			return CryptoEnginePKCS5.pbkdf2(masterKey, serialNumber, 2000, 32);
		}
		public static byte[] createCryptKey(byte[] serialNumber, string pin)
		{
			string _str = BaseFunctions.HexEncoder(serialNumber);
			return HashBaseFunction.createBinaryHash(_str.Substring(_str.Length - 4, 4) + pin + _str.Substring(0, 2));
		}
		public static string createActivationKey(byte pinCheckDigit)
		{
			byte[] buffer = BaseFunctions.secureRandom(14);
			buffer[0] = pinCheckDigit;
			string str = "";
			for (int i = 0; i < buffer.Length; i++)
			{
				str += (int)((buffer[i] % 10 < 0) ? (buffer[i] % 10 + 10) : (buffer[i] % 10));
			}
			string s = str.Substring(0, 7);
			string str2 = str.Substring(7, 7);
			string str3 = string.Concat(BaseFunctions.calcChecksum(long.Parse(s), 7));
			string str4 = string.Concat(BaseFunctions.calcChecksum(long.Parse(str2), 7));
			return string.Concat(new string[]
			{
				s,
				str3,
				"-",
				str2,
				str4
			});
		}
		public static byte[] createSeed(byte[] masterKey)
		{
			byte[] buffer = BaseFunctions.secureRandom(32);
			byte[] buffer2;
			if (masterKey == null)
			{
				buffer2 = new byte[buffer.Length];
				buffer.CopyTo(buffer2, 0);
			}
			else
			{
				buffer2 = new byte[masterKey.Length + buffer.Length];
				masterKey.CopyTo(buffer2, 0);
				buffer.CopyTo(buffer2, masterKey.Length);
			}
			return HashBaseFunction.createBinaryHash(buffer2);
		}
		public static byte[] createSeed(string pin, string activationKey)
		{
			return HOTPCipherInitialize.derivateKey(BaseFunctions.convertStringToByteArray(activationKey), BaseFunctions.convertStringToByteArray(pin), 10, 32);
		}
		public static long createSequenceNumber()
		{
			byte[] buffer = BaseFunctions.secureRandom(2);
			return (long)(buffer[0] * 10 + buffer[1]);
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
			RNGCryptoServiceProvider _RNGCrypto = new RNGCryptoServiceProvider();
			string _base;
			do
			{
				byte[] _buffer = new byte[2];
				_RNGCrypto.GetNonZeroBytes(_buffer);
				_base = Math.Abs(BitConverter.ToInt16(_buffer, 0)).ToString();
			}
			while (_base.Length != 4 || _base.StartsWith("00") || _base.EndsWith("00"));
			return _base;
		}
		public static string GeneratePassword(int size, bool lowerCase)
		{
			int seed = Environment.TickCount;
			Random _rnd = new Random();
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < size; i++)
			{
				char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * new Random(seed++).NextDouble() + 65.0)));
				builder.Append(ch);
			}
			return lowerCase ? builder.ToString().ToLower() : builder.ToString();
		}
		public static string GenerateNumericSerialNumber(int size)
		{
			Guid _guid = Guid.NewGuid();
			int _size = (size != 0) ? size : 32;
			string _guidTNumeric = Math.Abs(BitConverter.ToInt32(_guid.ToByteArray(), 0)).ToString().PadRight(_size - 1, '0');
			return _guidTNumeric + BaseFunctions.calcChecksum(long.Parse(_guidTNumeric), _size);
		}
		public static string GenerateComplexPassword(int size)
		{
			size = ((size > 16 || size < 6) ? 8 : size);
			byte[] data = new byte[size];
			StringBuilder result = new StringBuilder(size);
			char[] chars = "|!#$%&=?@*+abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
			new RNGCryptoServiceProvider().GetNonZeroBytes(data);
			byte[] array = data;
			for (int i = 0; i < array.Length; i++)
			{
				byte b = array[i];
				result.Append(chars[(int)b % (chars.Length - 1)]);
			}
			return result.ToString();
		}
	}
}
