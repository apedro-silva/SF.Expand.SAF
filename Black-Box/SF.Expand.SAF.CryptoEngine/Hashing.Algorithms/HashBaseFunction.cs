

using System;


namespace SF.Expand.SAF.CryptoEngine
{
    public static class HashBaseFunction
    {
        private const uint _H00 = 0x6a09e667;
        private const uint _H10 = 0xbb67ae85;
        private const uint _H20 = 0x3c6ef372;
        private const uint _H30 = 0xa54ff53a;
        private const uint _H40 = 0x510e527f;
        private const uint _H50 = 0x9b05688c;
        private const uint _H60 = 0x1f83d9ab;
        private const uint _H70 = 0x5be0cd19;


        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] createBinaryHash(string data)
        {
            return createBinaryHash(BaseFunctions.convertStringToByteArray(data));
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static byte[] createBinaryHash(byte[] message)
        {
            int num2;
            int n = message.Length >> 6;
            if ((message.Length & 0x3f) < 0x38)
            {
                n++;
                num2 = 0x38;
            }
            else
            {
                n += 2;
                num2 = 120;
            }
            long num3 = message.Length << 3;
            byte[] sourceArray = new byte[num2 - (message.Length & 0x3f)];
            sourceArray[0] = 0x80;
            for (int i = 1; i < sourceArray.Length; i++)
            {
                sourceArray[i] = 0;
            }
            byte[] destinationArray = new byte[n * 0x40];
            Array.Copy(message, 0, destinationArray, 0, message.Length);
            Array.Copy(sourceArray, 0, destinationArray, message.Length, sourceArray.Length);
            byte[] bytes = BitConverter.GetBytes(num3);
            byte[] buffer3 = new byte[8];
            for (int j = 0; j < 8; j++)
            {
                buffer3[j] = bytes[7 - j];
            }
            Array.Copy(buffer3, 0, destinationArray, message.Length + sourceArray.Length, 8);
            return _Hash256Bits(n, destinationArray);
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string createStringHash(byte[] data)
        {
            return BaseFunctions.convertByteArrayToString(createBinaryHash(data));
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string createStringHash(string data)
        {
            return BaseFunctions.convertByteArrayToString(createBinaryHash(BaseFunctions.convertStringToByteArray(data)));
        }

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private static uint Func(bool b, uint x, uint y, uint z)
        {
            if (b)
            {
                return ((x & y) ^ (~x & z));
            }
            return (((x & y) ^ (x & z)) ^ (y & z));
        }

        /// <summary>
        /// </summary>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        private static byte[] _Hash256Bits(int n, byte[] m)
        {
            uint[] numArray = new uint[] 
                                { 
                                    0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5, 0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174, 
                                    0xe49b69c1, 0xefbe4786, 0xfc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da, 0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x6ca6351, 0x14292967, 
                                    0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85, 0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070, 
                                    0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3, 0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
                                };
            uint[] numArray2 = new uint[8];
            uint[] numArray3 = new uint[0x40];
            byte[] destinationArray = new byte[0x20];
            byte[] buffer2 = new byte[4];
            byte[] bytes = new byte[4];
            numArray2[0] = 0x6a09e667;
            numArray2[1] = 0xbb67ae85;
            numArray2[2] = 0x3c6ef372;
            numArray2[3] = 0xa54ff53a;
            numArray2[4] = 0x510e527f;
            numArray2[5] = 0x9b05688c;
            numArray2[6] = 0x1f83d9ab;
            numArray2[7] = 0x5be0cd19;
            for (int i = 0; i < n; i++)
            {
                for (int k = 0; k < 0x40; k++)
                {
                    if (k < 0x10)
                    {
                        for (int num13 = 0; num13 < 4; num13++)
                        {
                            buffer2[num13] = m[(((i * 0x40) + (k * 4)) + 3) - num13];
                        }
                        numArray3[k] = BitConverter.ToUInt32(buffer2, 0);
                    }
                    else
                    {
                        numArray3[k] = ((_sigmaFunct(3, numArray3[k - 2]) + numArray3[k - 7]) + _sigmaFunct(2, numArray3[k - 15])) + numArray3[k - 0x10];
                    }
                }
                uint x = numArray2[0];
                uint y = numArray2[1];
                uint z = numArray2[2];
                uint num4 = numArray2[3];
                uint num5 = numArray2[4];
                uint num6 = numArray2[5];
                uint num7 = numArray2[6];
                uint num8 = numArray2[7];
                for (int num14 = 0; num14 < 0x40; num14++)
                {
                    uint num9 = (((num8 + _sigmaFunct(1, num5)) + Func(true, num5, num6, num7)) + numArray[num14]) + numArray3[num14];
                    uint num10 = _sigmaFunct(0, x) + Func(false, x, y, z);
                    num8 = num7;
                    num7 = num6;
                    num6 = num5;
                    num5 = num4 + num9;
                    num4 = z;
                    z = y;
                    y = x;
                    x = num9 + num10;
                }
                numArray2[0] = x + numArray2[0];
                numArray2[1] = y + numArray2[1];
                numArray2[2] = z + numArray2[2];
                numArray2[3] = num4 + numArray2[3];
                numArray2[4] = num5 + numArray2[4];
                numArray2[5] = num6 + numArray2[5];
                numArray2[6] = num7 + numArray2[6];
                numArray2[7] = num8 + numArray2[7];
            }
            for (int j = 0; j < 8; j++)
            {
                bytes = BitConverter.GetBytes(numArray2[j]);
                for (int num16 = 0; num16 < 4; num16++)
                {
                    buffer2[num16] = bytes[3 - num16];
                }
                Array.Copy(buffer2, 0, destinationArray, j * 4, 4);
            }
            return destinationArray;
        }

        /// <summary>
        /// </summary>
        /// <param name="i"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static uint _sigmaFunct(int i, uint x)
        {
            uint num;
            switch (i)
            {
                case 0:
                    num = (x >> 2) | (x << 30);
                    num ^= (x >> 13) | (x << 0x13);
                    return (num ^ ((x >> 0x16) | (x << 10)));

                case 1:
                    num = (x >> 6) | (x << 0x1a);
                    num ^= (x >> 11) | (x << 0x15);
                    return (num ^ ((x >> 0x19) | (x << 7)));

                case 2:
                    num = (x >> 7) | (x << 0x19);
                    num ^= (x >> 0x12) | (x << 14);
                    return (num ^ (x >> 3));

                case 3:
                    num = (x >> 0x11) | (x << 15);
                    num ^= (x >> 0x13) | (x << 13);
                    return (num ^ (x >> 10));
            }
            return x;
        }
    }
}