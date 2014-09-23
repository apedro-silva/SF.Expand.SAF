using System;
using System.Text;
namespace SF.Expand.SAF.Configuration
{
	public struct SecurityInfo
	{
		private byte[] _iv;
		private byte[] _key;
		private byte[] _masterKey;
		public byte[] MasterKey
		{
			get
			{
				return this._masterKey;
			}
		}
		public byte[] Key
		{
			get
			{
				return this._key;
			}
		}
		public byte[] Iv
		{
			get
			{
				return this._iv;
			}
		}
		public SecurityInfo(string masterKey, string key, string iv)
		{
			Encoding aSCII = Encoding.ASCII;
			Encoding unicode = Encoding.Unicode;
			this._key = Encoding.Convert(unicode, aSCII, unicode.GetBytes(key));
			this._iv = Encoding.Convert(unicode, aSCII, unicode.GetBytes(iv));
			this._masterKey = Encoding.Convert(unicode, aSCII, unicode.GetBytes(masterKey));
		}
		public SecurityInfo(byte[] masterKey, byte[] key, byte[] iv)
		{
			this._key = key;
			this._iv = iv;
			this._masterKey = masterKey;
		}
	}
}
