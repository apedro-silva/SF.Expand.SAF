using SF.Expand.SAF.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;
public class CryptorEngineRSA
{
	public static string Encrypt(string toEncrypt)
	{
		RSACryptoServiceProvider CSPRSA = null;
		string result;
		try
		{
			CSPRSA = new RSACryptoServiceProvider();
			CSPRSA.FromXmlString(SAFSecurityKeys.loadKeysFromFile());
			byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
			result = Convert.ToBase64String(CSPRSA.Decrypt(toEncryptArray, false));
		}
		catch
		{
			result = null;
		}
		finally
		{
			if (CSPRSA != null)
			{
				CSPRSA.Clear();
			}
		}
		return result;
	}
	public static string Decrypt(string toDecrypt)
	{
		RSACryptoServiceProvider CSPRSA = null;
		string result;
		try
		{
			CSPRSA = new RSACryptoServiceProvider();
			CSPRSA.FromXmlString(SAFSecurityKeys.loadKeysFromFile());
			byte[] toDecryptArray = Convert.FromBase64String(toDecrypt);
			result = Encoding.UTF8.GetString(CSPRSA.Decrypt(toDecryptArray, false));
		}
		catch
		{
			result = null;
		}
		finally
		{
			if (CSPRSA != null)
			{
				CSPRSA.Clear();
			}
		}
		return result;
	}
}
