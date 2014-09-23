using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
namespace SF.Expand.SAF.CryptoEngine
{
	public static class BaseFunctions
	{
		public static int calcChecksum(long num, int digits)
		{
			bool flag = true;
			int num2 = 0;
			int[] dbDigit = new int[]
			{
				0,
				2,
				4,
				6,
				8,
				1,
				3,
				5,
				7,
				9
			};
			while (0 < digits--)
			{
				int index = (int)(num % 10L);
				num /= 10L;
				if (flag)
				{
					index = dbDigit[index];
				}
				num2 += index;
				flag = !flag;
			}
			int num3 = num2 % 10;
			if (num3 > 0)
			{
				num3 = 10 - num3;
			}
			return num3;
		}
		public static string convertByteArrayToString(byte[] data)
		{
			int index = 0;
			char[] chArray = new char[data.Length];
			while (index < data.Length && data[index] != 0)
			{
				chArray[index] = (char)data[index];
				index++;
			}
			return new string(chArray, 0, index);
		}
		public static byte[] convertStringToByteArray(string text)
		{
			char[] chArray = text.ToCharArray();
			byte[] buffer = new byte[text.Length];
			for (int i = 0; i < text.Length; i++)
			{
				buffer[i] = (byte)chArray[i];
			}
			return buffer;
		}
		public static byte[] HexDecoder(string data)
		{
			if (data.Length % 2 != 0)
			{
				throw new Exception("Invalid hex value");
			}
			byte[] buffer = new byte[data.Length / 2];
			for (int i = 0; i < data.Length; i += 2)
			{
				buffer[i / 2] = byte.Parse(data.Substring(i, 2), NumberStyles.HexNumber);
			}
			return buffer;
		}
		public static string HexEncoder(byte[] data)
		{
			string str = "";
			for (int i = 0; i < data.Length; i++)
			{
				str += string.Format("{0:x2}", data[i]);
			}
			return str;
		}
		public static byte[] convertLongToArray(long num)
		{
			return new byte[]
			{
				(byte)(num & 255L),
				(byte)(num >> 8 & 255L),
				(byte)(num >> 16 & 255L),
				(byte)(num >> 24 & 255L),
				(byte)(num >> 32 & 255L),
				(byte)(num >> 40 & 255L),
				(byte)(num >> 48 & 255L),
				(byte)(num >> 56 & 255L)
			};
		}
		public static long convertArrayToLong(byte[] array)
		{
			long num = (long)(array[7] & 255);
			long num2 = (long)(array[6] & 255);
			long num3 = (long)(array[5] & 255);
			long num4 = (long)(array[4] & 255);
			long num5 = (long)(array[3] & 255);
			long num6 = (long)(array[2] & 255);
			long num7 = (long)(array[1] & 255);
			long num8 = (long)(array[0] & 255);
			return num8 << 56 | num7 << 48 | num6 << 40 | num5 << 32 | num4 << 24 | num3 << 16 | num2 << 8 | num;
		}
		public static byte convBCD(int value)
		{
			return byte.Parse(value.ToString(), NumberStyles.HexNumber);
		}
		public static string convBase(long input)
		{
			long _in = input;
			string _return = null;
			char[] _vetor = new char[]
			{
				'a',
				'b',
				'c',
				'd',
				'e',
				'f',
				'g',
				'h',
				'i',
				'j',
				'k',
				'l',
				'm',
				'n',
				'o',
				'p',
				'q',
				'r',
				's',
				't',
				'u',
				'v',
				'x',
				'w',
				'y',
				'z'
			};
			long ind;
			do
			{
				ind = _in / (long)(_vetor.Length - 1);
				long ind2 = _in % (long)(_vetor.Length - 1);
				_return = _vetor[(int)ind2] + _return;
				_in = ind;
			}
			while (ind > (long)(_vetor.Length - 1));
			return _vetor[(int)ind] + _return;
		}
		public static byte checkDigit(long num)
		{
			byte total = 0;
			bool doubleDigit = true;
			int digits = num.ToString().Length;
			byte[] doubleDigits = new byte[]
			{
				0,
				2,
				4,
				6,
				8,
				1,
				3,
				5,
				7,
				9
			};
			while (0 < digits--)
			{
				int digit = (int)(num % 10L);
				num /= 10L;
				if (doubleDigit)
				{
					digit = (int)doubleDigits[digit];
				}
				total += (byte)digit;
				doubleDigit = !doubleDigit;
			}
			byte result = (byte)(total % 10);
			if (result > 0)
			{
				result = (byte)(10 - result);
			}
			return result;
		}
		public static byte CreateRandomByte()
		{
			return BaseFunctions.secureRandom(1)[0];
		}
		public static byte[] secureRandom(int size)
		{
			byte[] data = new byte[size];
			new RNGCryptoServiceProvider().GetBytes(data);
			return data;
		}
		public static string EncodeTo64(string toEncode)
		{
			if (toEncode == null || toEncode.Length < 1)
			{
				return toEncode;
			}
			byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
			return Convert.ToBase64String(toEncodeAsBytes);
		}
		public static string DecodeFrom64(string encodedData)
		{
			if (encodedData == null || encodedData.Length < 1)
			{
				return encodedData;
			}
			byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
			return Encoding.ASCII.GetString(encodedDataAsBytes);
		}
		public static string GenerateSupplierLotNumber(string totalSeedsLot, string entropy)
		{
			if (totalSeedsLot == null)
			{
				return null;
			}
			if (entropy == null)
			{
				entropy = string.Empty;
			}
			DateTime now = DateTime.Now;
			return string.Concat(new string[]
			{
				now.ToString("yyddMM"),
				"-",
				entropy.Trim(),
				totalSeedsLot.Trim().PadLeft(7, '0'),
				"-",
				now.Ticks.ToString()
			});
		}
	}
}
