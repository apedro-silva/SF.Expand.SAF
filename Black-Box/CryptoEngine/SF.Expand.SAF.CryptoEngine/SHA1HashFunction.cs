using System;
namespace SF.Expand.SAF.CryptoEngine
{
	public class SHA1HashFunction
	{
		public static int BlockSize = 64;
		public static int HashSize = 20;
		private int Computed;
		private int Corrupted;
		private uint[] Intermediate_Hash = new uint[SHA1HashFunction.HashSize / 4];
		private int Length_High;
		private int Length_Low;
		private byte[] Message_Block = new byte[SHA1HashFunction.BlockSize];
		private int Message_Block_Index;
		public void Reset()
		{
			this.Length_Low = 0;
			this.Length_High = 0;
			this.Message_Block_Index = 0;
			this.Intermediate_Hash[0] = 1732584193u;
			this.Intermediate_Hash[1] = 4023233417u;
			this.Intermediate_Hash[2] = 2562383102u;
			this.Intermediate_Hash[3] = 271733878u;
			this.Intermediate_Hash[4] = 3285377520u;
			this.Computed = 0;
			this.Corrupted = 0;
		}
		public byte[] Result()
		{
			byte[] buffer = new byte[SHA1HashFunction.HashSize];
			if (this.Corrupted > 0)
			{
				throw new Exception("Corrupted.");
			}
			if (this.Computed == 0)
			{
				this.PadMessage();
				for (int num = 0; num < 64; num++)
				{
					this.Message_Block[num] = 0;
				}
				this.Length_Low = 0;
				this.Length_High = 0;
				this.Computed = 1;
			}
			for (int num = 0; num < SHA1HashFunction.HashSize; num++)
			{
				buffer[num] = (byte)(this.Intermediate_Hash[num >> 2] >> 8 * (3 - (num & 3)));
			}
			return buffer;
		}
		public bool Input(byte[] message_array)
		{
			int length = message_array.Length;
			bool result;
			if (message_array == null)
			{
				result = false;
			}
			else
			{
				if (length != 0)
				{
					if (this.Computed > 0)
					{
						this.Corrupted = 3;
						result = false;
						return result;
					}
					if (this.Corrupted > 0)
					{
						result = false;
						return result;
					}
					while (length-- > 0 && this.Corrupted == 0)
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
						if (this.Message_Block_Index == 64)
						{
							this.ProcessMessageBlock();
						}
					}
				}
				result = true;
			}
			return result;
		}
		private void PadMessage()
		{
			if (this.Message_Block_Index > 55)
			{
				this.Message_Block[this.Message_Block_Index++] = 128;
				while (this.Message_Block_Index < 64)
				{
					this.Message_Block[this.Message_Block_Index++] = 0;
				}
				this.ProcessMessageBlock();
				while (this.Message_Block_Index < 56)
				{
					this.Message_Block[this.Message_Block_Index++] = 0;
				}
			}
			else
			{
				this.Message_Block[this.Message_Block_Index++] = 128;
				while (this.Message_Block_Index < 56)
				{
					this.Message_Block[this.Message_Block_Index++] = 0;
				}
			}
			this.Message_Block[56] = (byte)(this.Length_High >> 24);
			this.Message_Block[57] = (byte)(this.Length_High >> 16);
			this.Message_Block[58] = (byte)(this.Length_High >> 8);
			this.Message_Block[59] = (byte)this.Length_High;
			this.Message_Block[60] = (byte)(this.Length_Low >> 24);
			this.Message_Block[61] = (byte)(this.Length_Low >> 16);
			this.Message_Block[62] = (byte)(this.Length_Low >> 8);
			this.Message_Block[63] = (byte)this.Length_Low;
			this.ProcessMessageBlock();
		}
		private static uint CircularShift(int bits, uint word)
		{
			return word << bits | word >> 32 - bits;
		}
		private void ProcessMessageBlock()
		{
			uint[] numArray = new uint[]
			{
				1518500249u,
				1859775393u,
				2400959708u,
				3395469782u
			};
			uint[] numArray2 = new uint[80];
			for (int num = 0; num < 16; num++)
			{
				numArray2[num] = (uint)((uint)this.Message_Block[num * 4] << 24);
				numArray2[num] |= (uint)((uint)this.Message_Block[num * 4 + 1] << 16);
				numArray2[num] |= (uint)((uint)this.Message_Block[num * 4 + 2] << 8);
				numArray2[num] |= (uint)this.Message_Block[num * 4 + 3];
			}
			for (int num = 16; num < 80; num++)
			{
				numArray2[num] = SHA1HashFunction.CircularShift(1, numArray2[num - 3] ^ numArray2[num - 8] ^ numArray2[num - 14] ^ numArray2[num - 16]);
			}
			uint word = this.Intermediate_Hash[0];
			uint num2 = this.Intermediate_Hash[1];
			uint num3 = this.Intermediate_Hash[2];
			uint num4 = this.Intermediate_Hash[3];
			uint num5 = this.Intermediate_Hash[4];
			for (int num = 0; num < 20; num++)
			{
				uint num6 = SHA1HashFunction.CircularShift(5, word) + ((num2 & num3) | (~num2 & num4)) + num5 + numArray2[num] + numArray[0];
				num5 = num4;
				num4 = num3;
				num3 = SHA1HashFunction.CircularShift(30, num2);
				num2 = word;
				word = num6;
			}
			for (int num = 20; num < 40; num++)
			{
				uint num6 = SHA1HashFunction.CircularShift(5, word) + (num2 ^ num3 ^ num4) + num5 + numArray2[num] + numArray[1];
				num5 = num4;
				num4 = num3;
				num3 = SHA1HashFunction.CircularShift(30, num2);
				num2 = word;
				word = num6;
			}
			for (int num = 40; num < 60; num++)
			{
				uint num6 = SHA1HashFunction.CircularShift(5, word) + ((num2 & num3) | (num2 & num4) | (num3 & num4)) + num5 + numArray2[num] + numArray[2];
				num5 = num4;
				num4 = num3;
				num3 = SHA1HashFunction.CircularShift(30, num2);
				num2 = word;
				word = num6;
			}
			for (int num = 60; num < 80; num++)
			{
				uint num6 = SHA1HashFunction.CircularShift(5, word) + (num2 ^ num3 ^ num4) + num5 + numArray2[num] + numArray[3];
				num5 = num4;
				num4 = num3;
				num3 = SHA1HashFunction.CircularShift(30, num2);
				num2 = word;
				word = num6;
			}
			this.Intermediate_Hash[0] += word;
			this.Intermediate_Hash[1] += num2;
			this.Intermediate_Hash[2] += num3;
			this.Intermediate_Hash[3] += num4;
			this.Intermediate_Hash[4] += num5;
			this.Message_Block_Index = 0;
		}
	}
}
