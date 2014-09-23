using SF.Expand.SAF.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;
namespace SF.Expand.SAF.CryptoEngine
{
	public static class CryptorEngineTripleDES
	{
		public static string Encrypt(string toEncrypt, SecurityInfo sInfo, bool useHashing)
		{
			TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
			byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
			if (useHashing)
			{
				tdes.Key = new MD5CryptoServiceProvider().ComputeHash(sInfo.Key);
			}
			tdes.Mode = CipherMode.ECB;
			tdes.Padding = PaddingMode.PKCS7;
			ICryptoTransform cTransform = tdes.CreateEncryptor();
			byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
			tdes.Clear();
			return Convert.ToBase64String(resultArray, 0, resultArray.Length);
		}
		public static string Decrypt(string cipherString, SecurityInfo sInfo, bool useHashing)
		{
			TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
			byte[] toEncryptArray = Convert.FromBase64String(cipherString);
			if (useHashing)
			{
				tdes.Key = new MD5CryptoServiceProvider().ComputeHash(sInfo.Key);
			}
			tdes.Mode = CipherMode.ECB;
			tdes.Padding = PaddingMode.PKCS7;
			ICryptoTransform cTransform = tdes.CreateDecryptor();
			byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
			tdes.Clear();
			return Encoding.UTF8.GetString(resultArray);
		}
	}
}
