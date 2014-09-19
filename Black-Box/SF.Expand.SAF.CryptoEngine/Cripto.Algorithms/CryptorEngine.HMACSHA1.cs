

using System;


namespace SF.Expand.SAF.CryptoEngine
{
    public static class CryptoEngineHMACSHA1
    {
        /// <summary>
        /// </summary>
        /// <param name="secretKey"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] process(byte[] secretKey, byte[] data, int length)
        {
            int num;
            SHA1HashFunction sha = new SHA1HashFunction();
            SHA1HashFunction sha2 = new SHA1HashFunction();
            byte[] buffer = new byte[SHA1HashFunction.HashSize];
            byte[] buffer2 = new byte[SHA1HashFunction.HashSize];
            byte[] buffer3 = new byte[SHA1HashFunction.HashSize];
            byte[] buffer4 = new byte[SHA1HashFunction.BlockSize];
            int hashSize = secretKey.Length;
            if (hashSize > SHA1HashFunction.BlockSize)
            {
                SHA1HashFunction sha3 = new SHA1HashFunction();
                sha3.Reset();
                sha3.Input(secretKey);
                secretKey = sha3.Result();
                hashSize = SHA1HashFunction.HashSize;
            }
            sha.Reset();
            for (num = 0; num < hashSize; num++)
            {
                buffer4[num] = (byte)(secretKey[num] ^ 0x36);
            }
            for (num = hashSize; num < SHA1HashFunction.BlockSize; num++)
            {
                buffer4[num] = 0x36;
            }
            sha.Input(buffer4);
            sha.Input(data);
            buffer = sha.Result();
            sha2.Reset();
            for (num = 0; num < hashSize; num++)
            {
                buffer4[num] = (byte)(secretKey[num] ^ 0x5c);
            }
            for (num = hashSize; num < SHA1HashFunction.BlockSize; num++)
            {
                buffer4[num] = 0x5c;
            }
            sha2.Input(buffer4);
            sha2.Input(buffer);
            buffer2 = sha2.Result();
            length = (length > SHA1HashFunction.HashSize) ? SHA1HashFunction.HashSize : length;
            byte[] buffer5 = new byte[length];
            truncate(buffer2, buffer5, length);
            return buffer5;
        }

        /// <summary>
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="len"></param>
        private static void truncate(byte[] d1, byte[] d2, int len)
        {
            for (int i = 0; i < len; i++)
            {
                d2[i] = d1[i];
            }
        }
    }
}