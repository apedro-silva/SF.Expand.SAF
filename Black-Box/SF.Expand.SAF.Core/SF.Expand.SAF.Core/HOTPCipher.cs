using SF.Expand.SAF.CryptoEngine;
using System;
namespace SF.Expand.SAF.Core
{
	public class HOTPCipher
	{
		private const string cINV_ARGS = "Invalid arguments!";
		public static byte[] decryptData(byte[] data, byte[] key)
		{
			return BaseFunctions.HexDecoder(BaseFunctions.convertByteArrayToString(HOTPCipher.decrypt(data, key)));
		}
		public static byte[] decrypt(byte[] data, byte[] key)
		{
			if (key == null || data == null || key.Length != 32)
			{
				throw new Exception("Invalid arguments!");
			}
			CryptoEngineAES _aesFunct = new CryptoEngineAES(CryptoEngineAES.KeySize.Bits256, key);
			byte[] destinationArray = new byte[data.Length];
			byte[] buffer2 = new byte[16];
			byte[] output = new byte[16];
			for (int i = 0; i < data.Length; i += 16)
			{
				Array.Copy(data, i, buffer2, 0, 16);
				_aesFunct.InvCipher(buffer2, output);
				Array.Copy(output, 0, destinationArray, i, 16);
			}
			return destinationArray;
		}
		public static byte[] encryptData(byte[] data, byte[] key)
		{
			char[] chArray = BaseFunctions.HexEncoder(data).ToCharArray();
			byte[] buffer = new byte[chArray.Length];
			for (int i = 0; i < chArray.Length; i++)
			{
				buffer[i] = (byte)chArray[i];
			}
			return HOTPCipher.encrypt(buffer, key);
		}
		public static byte[] encrypt(byte[] data, byte[] key)
		{
			if (key == null || data == null || key.Length != 32)
			{
				throw new Exception("Invalid arguments!");
			}
			int num = ((data.Length - 1) / 16 + 1) * 16;
			byte[] destinationArray = new byte[num];
			CryptoEngineAES _aesFunct = new CryptoEngineAES(CryptoEngineAES.KeySize.Bits256, key);
			byte[] buffer = new byte[data.Length];
			byte[] buffer2 = new byte[16];
			byte[] output = new byte[16];
			byte[] buffer3 = new byte[num];
			Array.Copy(data, buffer3, data.Length);
			for (int i = 0; i < buffer3.Length; i += 16)
			{
				Array.Copy(buffer3, i, buffer2, 0, 16);
				_aesFunct.Cipher(buffer2, output);
				Array.Copy(output, 0, destinationArray, i, 16);
			}
			return destinationArray;
		}
	}
}
