

using System;


namespace SF.Expand.SAF.CryptoEngine
{
    public static class CryptoEnginePKCS5
    {
        /// <summary>
        /// </summary>
        /// <param name="Password"></param>
        /// <param name="Salt"></param>
        /// <param name="Iterator"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static byte[] pbkdf2(byte[] Password, byte[] Salt, int Iterator, int Length)
        {
            int hashSize = SHA1HashFunction.HashSize;
            byte[] data = new byte[hashSize];
            byte[] sourceArray = new byte[hashSize];
            int num7 = Salt.Length + 4;
            byte[] destinationArray = new byte[Length];
            if (Length > (0x1d * hashSize))
            {
                throw new Exception("Derived key too long.");
            }
            if (Iterator <= 0)
            {
                throw new Exception("Invalid Iterator specified.");
            }
            if (Length <= 0)
            {
                throw new Exception("Invalid Length specified.");
            }
            int num3 = ((Length - 1) / hashSize) + 1;
            int num4 = Length - ((num3 - 1) * hashSize);
            byte[] buffer3 = new byte[num7];
            Array.Copy(Salt, 0, buffer3, 0, Salt.Length);

            for (int i = 1; i <= num3; i++)
            {
                sourceArray = new byte[hashSize];
                for (int j = 1; j <= Iterator; j++)
                {
                    if (j == 1)
                    {
                        buffer3[Salt.Length] = (byte)((i >> 0x18) & 0xff);
                        buffer3[Salt.Length + 1] = (byte)((i >> 0x10) & 0xff);
                        buffer3[Salt.Length + 2] = (byte)((i >> 8) & 0xff);
                        buffer3[Salt.Length + 3] = (byte)(i & 0xff);
                        data = CryptoEngineHMACSHA1.process(Password, buffer3, hashSize);
                    }
                    else
                    {
                        data = CryptoEngineHMACSHA1.process(Password, data, hashSize);
                    }
                    for (int k = 0; k < hashSize; k++)
                    {
                        sourceArray[k] = (byte)(sourceArray[k] ^ data[k]);
                    }
                }
                Array.Copy(sourceArray, 0, destinationArray, (i - 1) * hashSize, (i == num3) ? num4 : hashSize);
            }
            return destinationArray;
        }
    }
}
