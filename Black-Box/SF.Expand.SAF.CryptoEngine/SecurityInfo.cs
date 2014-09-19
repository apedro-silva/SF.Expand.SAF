using System;
using System.Text;
namespace SF.Expand.SAF.CryptoEngine
{
	[Serializable]
	public class SecurityInfo
	{
		private static byte[] _iv;
		private static byte[] _key;
		private static byte[] _masterKey;
		public byte[] MasterKey
		{
			get
			{
				return SecurityInfo._masterKey;
			}
		}
		public byte[] Key
		{
			get
			{
				return SecurityInfo._key;
			}
		}
		public byte[] Iv
		{
			get
			{
				return SecurityInfo._iv;
			}
		}
		public SecurityInfo()
		{
		}
		public SecurityInfo(string masterKey, string key, string iv)
		{
			Encoding _ascii = Encoding.ASCII;
			Encoding _unicode = Encoding.Unicode;
			SecurityInfo._key = ((key == null) ? null : Encoding.Convert(_unicode, _ascii, _unicode.GetBytes(key)));
			SecurityInfo._iv = ((iv == null) ? null : Encoding.Convert(_unicode, _ascii, _unicode.GetBytes(iv)));
			SecurityInfo._masterKey = ((masterKey == null) ? null : Encoding.Convert(_unicode, _ascii, _unicode.GetBytes(masterKey)));
		}
		public SecurityInfo(byte[] masterKey, byte[] key, byte[] iv)
		{
			SecurityInfo._key = key;
			SecurityInfo._iv = iv;
			SecurityInfo._masterKey = masterKey;
		}
	}
}
