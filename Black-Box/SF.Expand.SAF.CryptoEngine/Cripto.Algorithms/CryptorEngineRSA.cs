

using System;
using System.Text;
using System.IO;
using System.Configuration;
using System.Security.Cryptography;
using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.Utils;

namespace SF.Expand.SAF.CryptoEngine
{
    public class CryptorEngineRSA
    {
        /// <summary>
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <returns></returns>
        public static string Encrypt(string toEncrypt)
        {
            RSACryptoServiceProvider CSPRSA = null;
            try
            {
                CSPRSA = new RSACryptoServiceProvider();
                CSPRSA.FromXmlString(SAFSecurityKeys.loadKeysFromFile());
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
                return Convert.ToBase64String(CSPRSA.Decrypt(toEncryptArray, false));
            }
            catch
            {
                return null;
            }
            finally
            {
                if (CSPRSA != null)
                    CSPRSA.Clear();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="toDecrypt"></param>
        /// <returns></returns>
        public static string Decrypt(string toDecrypt)
        {
            RSACryptoServiceProvider CSPRSA = null;
            try
            {
                CSPRSA = new RSACryptoServiceProvider();
                CSPRSA.FromXmlString(SAFSecurityKeys.loadKeysFromFile());
                byte[] toDecryptArray = Convert.FromBase64String(toDecrypt);
                return Encoding.UTF8.GetString(CSPRSA.Decrypt(toDecryptArray, false));
            }
            catch
            {
                return null;
            }
            finally
            {
                if (CSPRSA != null)
                    CSPRSA.Clear();
            }
        }
    }
}