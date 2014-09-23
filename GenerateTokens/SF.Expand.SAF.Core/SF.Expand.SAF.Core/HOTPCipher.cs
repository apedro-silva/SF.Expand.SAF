using SF.Expand.SAF.CryptoEngine;
using System;
namespace SF.Expand.SAF.Core
{
	public class HOTPCipher
	{
		private const string cINV_ARGS = "Invalid arguments!";
		public static byte[] decrypt(byte[] data, byte[] key)
		{
			if (key == null || data == null || key.Length != 32)
			{
				throw new Exception("Invalid arguments!");
			}
			CryptoEngineAES cryptoEngineAES = new CryptoEngineAES(CryptoEngineAES.KeySize.Bits256, key);
			byte[] array = new byte[data.Length];
			byte[] array2 = new byte[16];
			byte[] array3 = new byte[16];
			for (int i = 0; i < data.Length; i += 16)
			{
				Array.Copy(data, i, array2, 0, 16);
				cryptoEngineAES.InvCipher(array2, array3);
				Array.Copy(array3, 0, array, i, 16);
			}
			return array;
		}
		public static byte[] decryptData(byte[] data, byte[] key)
		{
			return BaseFunctions.HexDecoder(BaseFunctions.convertByteArrayToString(HOTPCipher.decrypt(data, key)));
		}
		public static byte[] encrypt(byte[] data, byte[] key)
		{
			if (key == null || data == null || key.Length != 32)
			{
				throw new Exception("Invalid arguments!");
			}
			int num = ((data.Length - 1) / 16 + 1) * 16;
			byte[] array = new byte[num];
			CryptoEngineAES cryptoEngineAES = new CryptoEngineAES(CryptoEngineAES.KeySize.Bits256, key);
			int arg_38_0 = data.Length;
			byte[] array2 = new byte[16];
			byte[] array3 = new byte[16];
			byte[] array4 = new byte[num];
			Array.Copy(data, array4, data.Length);
			for (int i = 0; i < array4.Length; i += 16)
			{
				Array.Copy(array4, i, array2, 0, 16);
				cryptoEngineAES.Cipher(array2, array3);
				Array.Copy(array3, 0, array, i, 16);
			}
			return array;
		}
		public static byte[] encryptData(byte[] data, byte[] key)
		{
			char[] array = BaseFunctions.HexEncoder(data).ToCharArray();
			byte[] array2 = new byte[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = (byte)array[i];
			}
			return HOTPCipher.encrypt(array2, key);
		}
	}
}
