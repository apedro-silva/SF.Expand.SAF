

using System;


namespace SF.Expand.SAF.CryptoEngine
{
    public class CryptoEngineAES
    {
        private byte[,] iSbox;
        private byte[] key;
        private int Nb;
        private int Nk;
        private int Nr;
        private byte[,] Rcon;
        private byte[,] Sbox;
        private byte[,] State;
        private byte[,] w;

        public enum KeySize
        {
            Bits128,
            Bits192,
            Bits256
        }


        /// <summary>
        /// </summary>
        /// <param name="round"></param>
        private void AddRoundKey(int round)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.State[i, j] = (byte)(this.State[i, j] ^ this.w[(round * 4) + j, i]);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static byte gfmultby01(byte b)
        {
            return b;
        }

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static byte gfmultby02(byte b)
        {
            if (b < 0x80)
            {
                return (byte)(b << 1);
            }
            return (byte)((b << 1) ^ 0x1b);
        }

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static byte gfmultby03(byte b)
        {
            return (byte)(gfmultby02(b) ^ b);
        }

        //
        private static byte gfmultby09(byte b)
        {
            return (byte)(gfmultby02(gfmultby02(gfmultby02(b))) ^ b);
        }

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static byte gfmultby0b(byte b)
        {
            return (byte)((gfmultby02(gfmultby02(gfmultby02(b))) ^ gfmultby02(b)) ^ b);
        }

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static byte gfmultby0d(byte b)
        {
            return (byte)((gfmultby02(gfmultby02(gfmultby02(b))) ^ gfmultby02(gfmultby02(b))) ^ b);
        }

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static byte gfmultby0e(byte b)
        {
            return (byte)((gfmultby02(gfmultby02(gfmultby02(b))) ^ gfmultby02(gfmultby02(b))) ^ gfmultby02(b));
        }

        /// <summary>
        /// </summary>
        private void InvMixColumns()
        {
            int num2;
            byte[,] buffer = new byte[4, 4];
            for (int i = 0; i < 4; i++)
            {
                num2 = 0;
                while (num2 < 4)
                {
                    buffer[i, num2] = this.State[i, num2];
                    num2++;
                }
            }
            for (num2 = 0; num2 < 4; num2++)
            {
                this.State[0, num2] = (byte)(((gfmultby0e(buffer[0, num2]) ^ gfmultby0b(buffer[1, num2])) ^ gfmultby0d(buffer[2, num2])) ^ gfmultby09(buffer[3, num2]));
                this.State[1, num2] = (byte)(((gfmultby09(buffer[0, num2]) ^ gfmultby0e(buffer[1, num2])) ^ gfmultby0b(buffer[2, num2])) ^ gfmultby0d(buffer[3, num2]));
                this.State[2, num2] = (byte)(((gfmultby0d(buffer[0, num2]) ^ gfmultby09(buffer[1, num2])) ^ gfmultby0e(buffer[2, num2])) ^ gfmultby0b(buffer[3, num2]));
                this.State[3, num2] = (byte)(((gfmultby0b(buffer[0, num2]) ^ gfmultby0d(buffer[1, num2])) ^ gfmultby09(buffer[2, num2])) ^ gfmultby0e(buffer[3, num2]));
            }
        }

        /// <summary>
        /// </summary>
        private void InvShiftRows()
        {
            int num;
            int num2;
            byte[,] buffer = new byte[4, 4];
            for (num = 0; num < 4; num++)
            {
                num2 = 0;
                while (num2 < 4)
                {
                    buffer[num, num2] = this.State[num, num2];
                    num2++;
                }
            }
            for (num = 1; num < 4; num++)
            {
                for (num2 = 0; num2 < 4; num2++)
                {
                    this.State[num, (num2 + num) % this.Nb] = buffer[num, num2];
                }
            }
        }

        /// <summary>
        /// </summary>
        private void InvSubBytes()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.State[i, j] = this.iSbox[this.State[i, j] >> 4, this.State[i, j] & 15];
                }
            }
        }

        /// <summary>
        /// </summary>
        private void KeyExpansion()
        {
            int num;
            this.w = new byte[this.Nb * (this.Nr + 1), 4];
            for (num = 0; num < this.Nk; num++)
            {
                this.w[num, 0] = this.key[4 * num];
                this.w[num, 1] = this.key[(4 * num) + 1];
                this.w[num, 2] = this.key[(4 * num) + 2];
                this.w[num, 3] = this.key[(4 * num) + 3];
            }
            byte[] word = new byte[4];
            for (num = this.Nk; num < (this.Nb * (this.Nr + 1)); num++)
            {
                word[0] = this.w[num - 1, 0];
                word[1] = this.w[num - 1, 1];
                word[2] = this.w[num - 1, 2];
                word[3] = this.w[num - 1, 3];
                if ((num % this.Nk) == 0)
                {
                    word = this.SubWord(this.RotWord(word));
                    word[0] = (byte)(word[0] ^ this.Rcon[num / this.Nk, 0]);
                    word[1] = (byte)(word[1] ^ this.Rcon[num / this.Nk, 1]);
                    word[2] = (byte)(word[2] ^ this.Rcon[num / this.Nk, 2]);
                    word[3] = (byte)(word[3] ^ this.Rcon[num / this.Nk, 3]);
                }
                else if ((this.Nk > 6) && ((num % this.Nk) == 4))
                {
                    word = this.SubWord(word);
                }
                this.w[num, 0] = (byte)(this.w[num - this.Nk, 0] ^ word[0]);
                this.w[num, 1] = (byte)(this.w[num - this.Nk, 1] ^ word[1]);
                this.w[num, 2] = (byte)(this.w[num - this.Nk, 2] ^ word[2]);
                this.w[num, 3] = (byte)(this.w[num - this.Nk, 3] ^ word[3]);
            }
        }

        /// <summary>
        /// </summary>
        private void MixColumns()
        {
            int num2;
            byte[,] buffer = new byte[4, 4];
            for (int i = 0; i < 4; i++)
            {
                num2 = 0;
                while (num2 < 4)
                {
                    buffer[i, num2] = this.State[i, num2];
                    num2++;
                }
            }
            for (num2 = 0; num2 < 4; num2++)
            {
                this.State[0, num2] = (byte)(((gfmultby02(buffer[0, num2]) ^ gfmultby03(buffer[1, num2])) ^ gfmultby01(buffer[2, num2])) ^ gfmultby01(buffer[3, num2]));
                this.State[1, num2] = (byte)(((gfmultby01(buffer[0, num2]) ^ gfmultby02(buffer[1, num2])) ^ gfmultby03(buffer[2, num2])) ^ gfmultby01(buffer[3, num2]));
                this.State[2, num2] = (byte)(((gfmultby01(buffer[0, num2]) ^ gfmultby01(buffer[1, num2])) ^ gfmultby02(buffer[2, num2])) ^ gfmultby03(buffer[3, num2]));
                this.State[3, num2] = (byte)(((gfmultby03(buffer[0, num2]) ^ gfmultby01(buffer[1, num2])) ^ gfmultby01(buffer[2, num2])) ^ gfmultby02(buffer[3, num2]));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private byte[] RotWord(byte[] word)
        {
            return new byte[] { word[1], word[2], word[3], word[0] };
        }

        /// <summary>
        /// </summary>
        /// <param name="keySize"></param>
        private void SetNbNkNr(KeySize keySize)
        {
            this.Nb = 4;
            if (keySize == KeySize.Bits128)
            {
                this.Nk = 4;
                this.Nr = 10;
            }
            else if (keySize == KeySize.Bits192)
            {
                this.Nk = 6;
                this.Nr = 12;
            }
            else if (keySize == KeySize.Bits256)
            {
                this.Nk = 8;
                this.Nr = 14;
            }
        }

        /// <summary>
        /// </summary>
        private void ShiftRows()
        {
            int num;
            int num2;
            byte[,] buffer = new byte[4, 4];
            for (num = 0; num < 4; num++)
            {
                num2 = 0;
                while (num2 < 4)
                {
                    buffer[num, num2] = this.State[num, num2];
                    num2++;
                }
            }
            for (num = 1; num < 4; num++)
            {
                for (num2 = 0; num2 < 4; num2++)
                {
                    this.State[num, num2] = buffer[num, (num2 + num) % this.Nb];
                }
            }
        }

        /// <summary>
        /// </summary>
        private void SubBytes()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.State[i, j] = this.Sbox[this.State[i, j] >> 4, this.State[i, j] & 15];
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private byte[] SubWord(byte[] word)
        {
            return new byte[] { this.Sbox[word[0] >> 4, word[0] & 15], this.Sbox[word[1] >> 4, word[1] & 15], this.Sbox[word[2] >> 4, word[2] & 15], this.Sbox[word[3] >> 4, word[3] & 15] };
        }





        /// <summary>
        /// </summary>
        /// <param name="keySize"></param>
        /// <param name="keyBytes"></param>
        public CryptoEngineAES(KeySize keySize, byte[] keyBytes)
        {
            this.SetNbNkNr(keySize);
            this.key = new byte[this.Nk * 4];
            keyBytes.CopyTo(this.key, 0);
            this.Sbox = new byte[,] 
                        { 
                                { 0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 1, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76 }, 
                                { 0xca, 130, 0xc9, 0x7d, 250, 0x59, 0x47, 240, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0 }, 
                                { 0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15 }, 
                                { 4, 0xc7, 0x23, 0xc3, 0x18, 150, 5, 0x9a, 7, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75 }, 
                                { 9, 0x83, 0x2c, 0x1a, 0x1b, 110, 90, 160, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84 }, 
                                { 0x53, 0xd1, 0, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 190, 0x39, 0x4a, 0x4c, 0x58, 0xcf }, 
                                { 0xd0, 0xef, 170, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 2, 0x7f, 80, 60, 0x9f, 0xa8 }, 
                                { 0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 210 }, 
                                { 0xcd, 12, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 100, 0x5d, 0x19, 0x73 }, 
                                { 0x60, 0x81, 0x4f, 220, 0x22, 0x2a, 0x90, 0x88, 70, 0xee, 0xb8, 20, 0xde, 0x5e, 11, 0xdb }, 
                                { 0xe0, 50, 0x3a, 10, 0x49, 6, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79 }, 
                                { 0xe7, 200, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 8 }, 
                                { 0xba, 120, 0x25, 0x2e, 0x1c, 0xa6, 180, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a }, 
                                { 0x70, 0x3e, 0xb5, 0x66, 0x48, 3, 0xf6, 14, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e }, 
                                { 0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 30, 0x87, 0xe9, 0xce, 0x55, 40, 0xdf }, 
                                { 140, 0xa1, 0x89, 13, 0xbf, 230, 0x42, 0x68, 0x41, 0x99, 0x2d, 15, 0xb0, 0x54, 0xbb, 0x16 } 
                        };
            this.iSbox = new byte[,] 
                        { 
                                { 0x52, 9, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb }, 
                                { 0x7c, 0xe3, 0x39, 130, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb }, 
                                { 0x54, 0x7b, 0x94, 50, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 11, 0x42, 250, 0xc3, 0x4e }, 
                                { 8, 0x2e, 0xa1, 0x66, 40, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25 }, 
                                { 0x72, 0xf8, 0xf6, 100, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92 }, 
                                { 0x6c, 0x70, 0x48, 80, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 70, 0x57, 0xa7, 0x8d, 0x9d, 0x84 }, 
                                { 0x90, 0xd8, 0xab, 0, 140, 0xbc, 0xd3, 10, 0xf7, 0xe4, 0x58, 5, 0xb8, 0xb3, 0x45, 6 }, 
                                { 0xd0, 0x2c, 30, 0x8f, 0xca, 0x3f, 15, 2, 0xc1, 0xaf, 0xbd, 3, 1, 0x13, 0x8a, 0x6b }, 
                                { 0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 220, 0xea, 0x97, 0xf2, 0xcf, 0xce, 240, 180, 230, 0x73 }, 
                                { 150, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 110 }, 
                                { 0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 14, 170, 0x18, 190, 0x1b }, 
                                { 0xfc, 0x56, 0x3e, 0x4b, 0xc6, 210, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 120, 0xcd, 90, 0xf4 }, 
                                { 0x1f, 0xdd, 0xa8, 0x33, 0x88, 7, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f }, 
                                { 0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 13, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef }, 
                                { 160, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 200, 0xeb, 0xbb, 60, 0x83, 0x53, 0x99, 0x61 }, 
                                { 0x17, 0x2b, 4, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 20, 0x63, 0x55, 0x21, 12, 0x7d } 
                        };
            this.Rcon = new byte[,] 
                        { 
                                { 0, 0, 0, 0 }, 
                                { 1, 0, 0, 0 }, 
                                { 2, 0, 0, 0 }, 
                                { 4, 0, 0, 0 }, 
                                { 8, 0, 0, 0 }, 
                                { 0x10, 0, 0, 0 }, 
                                { 0x20, 0, 0, 0 }, 
                                { 0x40, 0, 0, 0 }, 
                                { 0x80, 0, 0, 0 }, 
                                { 0x1b, 0, 0, 0 }, 
                                { 0x36, 0, 0, 0 } 
                        };
            this.KeyExpansion();
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void Cipher(byte[] input, byte[] output)
        {
            int num;
            this.State = new byte[4, this.Nb];
            for (num = 0; num < (4 * this.Nb); num++)
            {
                this.State[num % 4, num / 4] = input[num];
            }
            this.AddRoundKey(0);
            for (int i = 1; i <= (this.Nr - 1); i++)
            {
                this.SubBytes();
                this.ShiftRows();
                this.MixColumns();
                this.AddRoundKey(i);
            }
            this.SubBytes();
            this.ShiftRows();
            this.AddRoundKey(this.Nr);
            for (num = 0; num < (4 * this.Nb); num++)
            {
                output[num] = this.State[num % 4, num / 4];
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void InvCipher(byte[] input, byte[] output)
        {
            int num;
            this.State = new byte[4, this.Nb];
            for (num = 0; num < (4 * this.Nb); num++)
            {
                this.State[num % 4, num / 4] = input[num];
            }
            this.AddRoundKey(this.Nr);
            for (int i = this.Nr - 1; i >= 1; i--)
            {
                this.InvShiftRows();
                this.InvSubBytes();
                this.AddRoundKey(i);
                this.InvMixColumns();
            }
            this.InvShiftRows();
            this.InvSubBytes();
            this.AddRoundKey(0);
            for (num = 0; num < (4 * this.Nb); num++)
            {
                output[num] = this.State[num % 4, num / 4];
            }
        }
    }
}