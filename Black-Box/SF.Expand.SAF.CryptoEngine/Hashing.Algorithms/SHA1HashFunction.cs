

using System;


namespace SF.Expand.SAF.CryptoEngine
{
    public class SHA1HashFunction
    {
        public static int BlockSize = 0x40;
        public static int HashSize = 20;
        private int Computed;
        private int Corrupted;
        private uint[] Intermediate_Hash = new uint[HashSize / 4];
        private int Length_High;
        private int Length_Low;
        private byte[] Message_Block = new byte[BlockSize];
        private int Message_Block_Index;


        /// <summary>
        /// </summary>
        public void Reset()
        {
            this.Length_Low = 0;
            this.Length_High = 0;
            this.Message_Block_Index = 0;
            this.Intermediate_Hash[0] = 0x67452301;
            this.Intermediate_Hash[1] = 0xefcdab89;
            this.Intermediate_Hash[2] = 0x98badcfe;
            this.Intermediate_Hash[3] = 0x10325476;
            this.Intermediate_Hash[4] = 0xc3d2e1f0;
            this.Computed = 0;
            this.Corrupted = 0;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public byte[] Result()
        {
            int num;
            byte[] buffer = new byte[HashSize];
            if (this.Corrupted > 0)
            {
                throw new Exception("Corrupted.");
            }
            if (this.Computed == 0)
            {
                this.PadMessage();
                for (num = 0; num < 0x40; num++)
                {
                    this.Message_Block[num] = 0;
                }
                this.Length_Low = 0;
                this.Length_High = 0;
                this.Computed = 1;
            }
            for (num = 0; num < HashSize; num++)
            {
                buffer[num] = (byte)(this.Intermediate_Hash[num >> 2] >> (8 * (3 - (num & 3))));
            }
            return buffer;
        }

        /// <summary>
        /// </summary>
        /// <param name="message_array"></param>
        /// <returns></returns>
        public bool Input(byte[] message_array)
        {
            int length = message_array.Length;
            if (message_array == null)
            {
                return false;
            }
            if (length != 0)
            {
                if (this.Computed > 0)
                {
                    this.Corrupted = 3;
                    return false;
                }
                if (this.Corrupted > 0)
                {
                    return false;
                }
                while ((length-- > 0) && (this.Corrupted == 0))
                {
                    this.Message_Block[this.Message_Block_Index] = message_array[this.Message_Block_Index++];
                    this.Length_Low += 8;
                    if (this.Length_Low == 0)
                    {
                        this.Length_High++;
                        if (this.Length_High == 0)
                        {
                            this.Corrupted = 1;
                        }
                    }
                    if (this.Message_Block_Index == 0x40)
                    {
                        this.ProcessMessageBlock();
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// </summary>
        private void PadMessage()
        {
            if (this.Message_Block_Index > 0x37)
            {
                this.Message_Block[this.Message_Block_Index++] = 0x80;
                while (this.Message_Block_Index < 0x40)
                {
                    this.Message_Block[this.Message_Block_Index++] = 0;
                }
                this.ProcessMessageBlock();
                while (this.Message_Block_Index < 0x38)
                {
                    this.Message_Block[this.Message_Block_Index++] = 0;
                }
            }
            else
            {
                this.Message_Block[this.Message_Block_Index++] = 0x80;
                while (this.Message_Block_Index < 0x38)
                {
                    this.Message_Block[this.Message_Block_Index++] = 0;
                }
            }
            this.Message_Block[0x38] = (byte)(this.Length_High >> 0x18);
            this.Message_Block[0x39] = (byte)(this.Length_High >> 0x10);
            this.Message_Block[0x3a] = (byte)(this.Length_High >> 8);
            this.Message_Block[0x3b] = (byte)this.Length_High;
            this.Message_Block[60] = (byte)(this.Length_Low >> 0x18);
            this.Message_Block[0x3d] = (byte)(this.Length_Low >> 0x10);
            this.Message_Block[0x3e] = (byte)(this.Length_Low >> 8);
            this.Message_Block[0x3f] = (byte)this.Length_Low;
            this.ProcessMessageBlock();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bits"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        private static uint CircularShift(int bits, uint word)
        {
            return ((word << bits) | (word >> (0x20 - bits)));
        }

        /// <summary>
        /// </summary>
        private void ProcessMessageBlock()
        {
            int num;
            uint num2;
            uint[] numArray = new uint[] { 0x5a827999, 0x6ed9eba1, 0x8f1bbcdc, 0xca62c1d6 };
            uint[] numArray2 = new uint[80];
            for (num = 0; num < 0x10; num++)
            {
                numArray2[num] = (uint)(this.Message_Block[num * 4] << 0x18);
                numArray2[num] |= (uint)(this.Message_Block[(num * 4) + 1] << 0x10);
                numArray2[num] |= (uint)(this.Message_Block[(num * 4) + 2] << 8);
                numArray2[num] |= this.Message_Block[(num * 4) + 3];
            }
            for (num = 0x10; num < 80; num++)
            {
                numArray2[num] = CircularShift(1, ((numArray2[num - 3] ^ numArray2[num - 8]) ^ numArray2[num - 14]) ^ numArray2[num - 0x10]);
            }
            uint word = this.Intermediate_Hash[0];
            uint num4 = this.Intermediate_Hash[1];
            uint num5 = this.Intermediate_Hash[2];
            uint num6 = this.Intermediate_Hash[3];
            uint num7 = this.Intermediate_Hash[4];
            for (num = 0; num < 20; num++)
            {
                num2 = (((CircularShift(5, word) + ((num4 & num5) | (~num4 & num6))) + num7) + numArray2[num]) + numArray[0];
                num7 = num6;
                num6 = num5;
                num5 = CircularShift(30, num4);
                num4 = word;
                word = num2;
            }
            for (num = 20; num < 40; num++)
            {
                num2 = (((CircularShift(5, word) + ((num4 ^ num5) ^ num6)) + num7) + numArray2[num]) + numArray[1];
                num7 = num6;
                num6 = num5;
                num5 = CircularShift(30, num4);
                num4 = word;
                word = num2;
            }
            for (num = 40; num < 60; num++)
            {
                num2 = (((CircularShift(5, word) + (((num4 & num5) | (num4 & num6)) | (num5 & num6))) + num7) + numArray2[num]) + numArray[2];
                num7 = num6;
                num6 = num5;
                num5 = CircularShift(30, num4);
                num4 = word;
                word = num2;
            }
            for (num = 60; num < 80; num++)
            {
                num2 = (((CircularShift(5, word) + ((num4 ^ num5) ^ num6)) + num7) + numArray2[num]) + numArray[3];
                num7 = num6;
                num6 = num5;
                num5 = CircularShift(30, num4);
                num4 = word;
                word = num2;
            }
            this.Intermediate_Hash[0] += word;
            this.Intermediate_Hash[1] += num4;
            this.Intermediate_Hash[2] += num5;
            this.Intermediate_Hash[3] += num6;
            this.Intermediate_Hash[4] += num7;
            this.Message_Block_Index = 0;
        }
    }
}
