using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace SF.Expand.SAF.CryptoEngine
{
	public class CryptorEngineRSA
	{
		internal static bool GetKey(bool isServer, out RSAParameters CSPRSAPARAM)
		{
			CSPRSAPARAM = default(RSAParameters);
			RSACryptoServiceProvider rsaCrypt = null;
			bool result;
			try
			{
				string _rsaKey = ConfigurationSettings.AppSettings["RSAKey"].ToString();
				if (_rsaKey != string.Empty)
				{
					rsaCrypt = new RSACryptoServiceProvider();
					rsaCrypt.FromXmlString(_rsaKey.Replace("(", "<").Replace(")", ">"));
					CSPRSAPARAM = rsaCrypt.ExportParameters(isServer);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (Exception ex)
			{
				string _msg = ex.ToString();
				result = false;
			}
			finally
			{
				if (rsaCrypt != null)
				{
					rsaCrypt.Clear();
				}
				rsaCrypt = null;
			}
			return result;
		}
		internal static string _loadKeysFromFile(string pathKeysFile)
		{
			FileStream pFSmInput = null;
			string result;
			try
			{
				pFSmInput = new FileStream(pathKeysFile, FileMode.Open, FileAccess.Read);
				byte[] pBytData = new byte[pFSmInput.Length];
				pFSmInput.Read(pBytData, 0, (int)pFSmInput.Length);
				result = Encoding.ASCII.GetString(pBytData);
			}
			catch (Exception ex)
			{
				string _msg = ex.ToString();
				result = null;
			}
			finally
			{
				if (pFSmInput != null)
				{
					pFSmInput.Close();
					pFSmInput = null;
				}
			}
			return result;
		}
		public static string Encrypt(string toEncrypt, string pathKeysFile)
		{
			RSACryptoServiceProvider CSPRSA = null;
			string result;
			try
			{
				CSPRSA = new RSACryptoServiceProvider();
				byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
				if (pathKeysFile != null)
				{
					CSPRSA.FromXmlString(CryptorEngineRSA._loadKeysFromFile(pathKeysFile));
					result = Convert.ToBase64String(CSPRSA.Encrypt(toEncryptArray, false));
				}
				else
				{
					RSAParameters CSPRSAPARAM;
					if (!CryptorEngineRSA.GetKey(true, out CSPRSAPARAM))
					{
						result = null;
					}
					else
					{
						CSPRSA.ImportParameters(CSPRSAPARAM);
						result = Convert.ToBase64String(CSPRSA.Encrypt(toEncryptArray, false));
					}
				}
			}
			catch (CryptographicException ex)
			{
				string _msg = ex.ToString();
				result = null;
			}
			finally
			{
				if (CSPRSA != null)
				{
					CSPRSA.Clear();
				}
				CSPRSA = null;
			}
			return result;
		}
		public static string Decrypt(string toDecrypt, string pathKeysFile)
		{
			RSACryptoServiceProvider CSPRSA = null;
			string result;
			try
			{
				CSPRSA = new RSACryptoServiceProvider();
				if (pathKeysFile != null)
				{
					CSPRSA.FromXmlString(CryptorEngineRSA._loadKeysFromFile(pathKeysFile));
					result = Convert.ToBase64String(CSPRSA.Decrypt(Encoding.UTF8.GetBytes(toDecrypt), false));
				}
				else
				{
					RSAParameters CSPRSAPARAM;
					if (!CryptorEngineRSA.GetKey(true, out CSPRSAPARAM))
					{
						result = null;
					}
					else
					{
						CSPRSA.ImportParameters(CSPRSAPARAM);
						result = Convert.ToBase64String(CSPRSA.Decrypt(Encoding.UTF8.GetBytes(toDecrypt), false));
					}
				}
			}
			catch (CryptographicException ex)
			{
				string _msg = ex.ToString();
				result = null;
			}
			finally
			{
				if (CSPRSA != null)
				{
					CSPRSA.Clear();
				}
				CSPRSA = null;
			}
			return result;
		}
	}
}
