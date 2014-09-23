using System;
using System.IO;
using System.Text;
namespace SF.Expand.SAF.Configuration
{
	public static class SAFSecurityKeys
	{
		public static SecurityInfo getSecurityInfoFromWConfig()
		{
			return new SecurityInfo(SAFConfiguration.readMasterKey(), SAFConfiguration.readInfoKey(), SAFConfiguration.readInfoIV());
		}
		public static string loadKeysFromFile()
		{
			FileStream fileStream = null;
			string result;
			try
			{
				fileStream = new FileStream(SAFConfiguration.readParameter("SAFKeyParsFilePath"), FileMode.Open, FileAccess.Read);
				byte[] array = new byte[fileStream.Length];
				fileStream.Read(array, 0, (int)fileStream.Length);
				string @string = Encoding.ASCII.GetString(array);
				fileStream.Close();
				result = @string;
			}
			catch
			{
				result = null;
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
			return result;
		}
		private static byte[] _prepareKey(string masterKey)
		{
			if (masterKey == null || masterKey.Length < 1)
			{
				return new byte[0];
			}
			byte[] result;
			try
			{
				long num = 0L;
				long num2 = 255L;
				long num3 = (long)masterKey.Length;
				byte[] array = new byte[num2];
				Encoding aSCII = Encoding.ASCII;
				Encoding unicode = Encoding.Unicode;
				byte[] array2 = Encoding.Convert(unicode, aSCII, unicode.GetBytes(masterKey));
				char[] array3 = new char[aSCII.GetCharCount(array2, 0, array2.Length)];
				aSCII.GetChars(array2, 0, array2.Length, array3, 0);
				for (long num4 = 0L; num4 < num2; num4 += 1L)
				{
					array[(int)checked((IntPtr)num4)] = (byte)num4;
				}
				for (long num5 = 0L; num5 < num2; num5 += 1L)
				{
					num = (num + (long)((ulong)array[(int)checked((IntPtr)num5)]) + (long)((ulong)array3[(int)checked((IntPtr)(num5 % num3))])) % num2;
					checked
					{
						byte b = array[(int)((IntPtr)num5)];
						array[(int)((IntPtr)num5)] = array[(int)((IntPtr)num)];
						array[(int)((IntPtr)num)] = b;
					}
				}
				result = array;
			}
			catch
			{
				result = new byte[0];
			}
			return result;
		}
	}
}
