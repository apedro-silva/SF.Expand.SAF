using System;
namespace SF.Expand.SAF.CryptoEngine
{
	public class CryptoEngineAES
	{
		public enum KeySize
		{
			Bits128,
			Bits192,
			Bits256
		}
		private byte[,] iSbox;
		private byte[] key;
		private int Nb;
		private int Nk;
		private int Nr;
		private byte[,] Rcon;
		private byte[,] Sbox;
		private byte[,] State;
		private byte[,] w;
		public CryptoEngineAES(CryptoEngineAES.KeySize keySize, byte[] keyBytes)
		{
			this.SetNbNkNr(keySize);
			this.key = new byte[this.Nk * 4];
			keyBytes.CopyTo(this.key, 0);
			this.Sbox = new byte[,]
			{

				{
					99,
					124,
					119,
					123,
					242,
					107,
					111,
					197,
					48,
					1,
					103,
					43,
					254,
					215,
					171,
					118
				},

				{
					202,
					130,
					201,
					125,
					250,
					89,
					71,
					240,
					173,
					212,
					162,
					175,
					156,
					164,
					114,
					192
				},

				{
					183,
					253,
					147,
					38,
					54,
					63,
					247,
					204,
					52,
					165,
					229,
					241,
					113,
					216,
					49,
					21
				},

				{
					4,
					199,
					35,
					195,
					24,
					150,
					5,
					154,
					7,
					18,
					128,
					226,
					235,
					39,
					178,
					117
				},

				{
					9,
					131,
					44,
					26,
					27,
					110,
					90,
					160,
					82,
					59,
					214,
					179,
					41,
					227,
					47,
					132
				},

				{
					83,
					209,
					0,
					237,
					32,
					252,
					177,
					91,
					106,
					203,
					190,
					57,
					74,
					76,
					88,
					207
				},

				{
					208,
					239,
					170,
					251,
					67,
					77,
					51,
					133,
					69,
					249,
					2,
					127,
					80,
					60,
					159,
					168
				},

				{
					81,
					163,
					64,
					143,
					146,
					157,
					56,
					245,
					188,
					182,
					218,
					33,
					16,
					255,
					243,
					210
				},

				{
					205,
					12,
					19,
					236,
					95,
					151,
					68,
					23,
					196,
					167,
					126,
					61,
					100,
					93,
					25,
					115
				},

				{
					96,
					129,
					79,
					220,
					34,
					42,
					144,
					136,
					70,
					238,
					184,
					20,
					222,
					94,
					11,
					219
				},

				{
					224,
					50,
					58,
					10,
					73,
					6,
					36,
					92,
					194,
					211,
					172,
					98,
					145,
					149,
					228,
					121
				},

				{
					231,
					200,
					55,
					109,
					141,
					213,
					78,
					169,
					108,
					86,
					244,
					234,
					101,
					122,
					174,
					8
				},

				{
					186,
					120,
					37,
					46,
					28,
					166,
					180,
					198,
					232,
					221,
					116,
					31,
					75,
					189,
					139,
					138
				},

				{
					112,
					62,
					181,
					102,
					72,
					3,
					246,
					14,
					97,
					53,
					87,
					185,
					134,
					193,
					29,
					158
				},

				{
					225,
					248,
					152,
					17,
					105,
					217,
					142,
					148,
					155,
					30,
					135,
					233,
					206,
					85,
					40,
					223
				},

				{
					140,
					161,
					137,
					13,
					191,
					230,
					66,
					104,
					65,
					153,
					45,
					15,
					176,
					84,
					187,
					22
				}
			};
			this.iSbox = new byte[,]
			{

				{
					82,
					9,
					106,
					213,
					48,
					54,
					165,
					56,
					191,
					64,
					163,
					158,
					129,
					243,
					215,
					251
				},

				{
					124,
					227,
					57,
					130,
					155,
					47,
					255,
					135,
					52,
					142,
					67,
					68,
					196,
					222,
					233,
					203
				},

				{
					84,
					123,
					148,
					50,
					166,
					194,
					35,
					61,
					238,
					76,
					149,
					11,
					66,
					250,
					195,
					78
				},

				{
					8,
					46,
					161,
					102,
					40,
					217,
					36,
					178,
					118,
					91,
					162,
					73,
					109,
					139,
					209,
					37
				},

				{
					114,
					248,
					246,
					100,
					134,
					104,
					152,
					22,
					212,
					164,
					92,
					204,
					93,
					101,
					182,
					146
				},

				{
					108,
					112,
					72,
					80,
					253,
					237,
					185,
					218,
					94,
					21,
					70,
					87,
					167,
					141,
					157,
					132
				},

				{
					144,
					216,
					171,
					0,
					140,
					188,
					211,
					10,
					247,
					228,
					88,
					5,
					184,
					179,
					69,
					6
				},

				{
					208,
					44,
					30,
					143,
					202,
					63,
					15,
					2,
					193,
					175,
					189,
					3,
					1,
					19,
					138,
					107
				},

				{
					58,
					145,
					17,
					65,
					79,
					103,
					220,
					234,
					151,
					242,
					207,
					206,
					240,
					180,
					230,
					115
				},

				{
					150,
					172,
					116,
					34,
					231,
					173,
					53,
					133,
					226,
					249,
					55,
					232,
					28,
					117,
					223,
					110
				},

				{
					71,
					241,
					26,
					113,
					29,
					41,
					197,
					137,
					111,
					183,
					98,
					14,
					170,
					24,
					190,
					27
				},

				{
					252,
					86,
					62,
					75,
					198,
					210,
					121,
					32,
					154,
					219,
					192,
					254,
					120,
					205,
					90,
					244
				},

				{
					31,
					221,
					168,
					51,
					136,
					7,
					199,
					49,
					177,
					18,
					16,
					89,
					39,
					128,
					236,
					95
				},

				{
					96,
					81,
					127,
					169,
					25,
					181,
					74,
					13,
					45,
					229,
					122,
					159,
					147,
					201,
					156,
					239
				},

				{
					160,
					224,
					59,
					77,
					174,
					42,
					245,
					176,
					200,
					235,
					187,
					60,
					131,
					83,
					153,
					97
				},

				{
					23,
					43,
					4,
					126,
					186,
					119,
					214,
					38,
					225,
					105,
					20,
					99,
					85,
					33,
					12,
					125
				}
			};
			this.Rcon = new byte[,]
			{

				{
					0,
					0,
					0,
					0
				},

				{
					1,
					0,
					0,
					0
				},

				{
					2,
					0,
					0,
					0
				},

				{
					4,
					0,
					0,
					0
				},

				{
					8,
					0,
					0,
					0
				},

				{
					16,
					0,
					0,
					0
				},

				{
					32,
					0,
					0,
					0
				},

				{
					64,
					0,
					0,
					0
				},

				{
					128,
					0,
					0,
					0
				},

				{
					27,
					0,
					0,
					0
				},

				{
					54,
					0,
					0,
					0
				}
			};
			this.KeyExpansion();
		}
		public void Cipher(byte[] input, byte[] output)
		{
			this.State = new byte[4, this.Nb];
			for (int num = 0; num < 4 * this.Nb; num++)
			{
				this.State[num % 4, num / 4] = input[num];
			}
			this.AddRoundKey(0);
			for (int i = 1; i <= this.Nr - 1; i++)
			{
				this.SubBytes();
				this.ShiftRows();
				this.MixColumns();
				this.AddRoundKey(i);
			}
			this.SubBytes();
			this.ShiftRows();
			this.AddRoundKey(this.Nr);
			for (int num = 0; num < 4 * this.Nb; num++)
			{
				output[num] = this.State[num % 4, num / 4];
			}
		}
		private void AddRoundKey(int round)
		{
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					this.State[i, j] = (byte)(this.State[i, j] ^ this.w[round * 4 + j, i]);
				}
			}
		}
		private static byte gfmultby01(byte b)
		{
			return b;
		}
		private static byte gfmultby02(byte b)
		{
			if (b < 128)
			{
				return (byte)(b << 1);
			}
			return (byte)((int)b << 1 ^ 27);
		}
		private static byte gfmultby03(byte b)
		{
			return (byte)(CryptoEngineAES.gfmultby02(b) ^ b);
		}
		private static byte gfmultby09(byte b)
		{
			return (byte)(CryptoEngineAES.gfmultby02(CryptoEngineAES.gfmultby02(CryptoEngineAES.gfmultby02(b))) ^ b);
		}
		private static byte gfmultby0b(byte b)
		{
			return (byte)(CryptoEngineAES.gfmultby02(CryptoEngineAES.gfmultby02(CryptoEngineAES.gfmultby02(b))) ^ CryptoEngineAES.gfmultby02(b) ^ b);
		}
		private static byte gfmultby0d(byte b)
		{
			return (byte)(CryptoEngineAES.gfmultby02(CryptoEngineAES.gfmultby02(CryptoEngineAES.gfmultby02(b))) ^ CryptoEngineAES.gfmultby02(CryptoEngineAES.gfmultby02(b)) ^ b);
		}
		private static byte gfmultby0e(byte b)
		{
			return (byte)(CryptoEngineAES.gfmultby02(CryptoEngineAES.gfmultby02(CryptoEngineAES.gfmultby02(b))) ^ CryptoEngineAES.gfmultby02(CryptoEngineAES.gfmultby02(b)) ^ CryptoEngineAES.gfmultby02(b));
		}
		public void InvCipher(byte[] input, byte[] output)
		{
			this.State = new byte[4, this.Nb];
			for (int num = 0; num < 4 * this.Nb; num++)
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
			for (int num = 0; num < 4 * this.Nb; num++)
			{
				output[num] = this.State[num % 4, num / 4];
			}
		}
		private void InvMixColumns()
		{
			byte[,] buffer = new byte[4, 4];
			for (int i = 0; i < 4; i++)
			{
				for (int num2 = 0; num2 < 4; num2++)
				{
					buffer[i, num2] = this.State[i, num2];
				}
			}
			for (int num2 = 0; num2 < 4; num2++)
			{
				this.State[0, num2] = (byte)(CryptoEngineAES.gfmultby0e(buffer[0, num2]) ^ CryptoEngineAES.gfmultby0b(buffer[1, num2]) ^ CryptoEngineAES.gfmultby0d(buffer[2, num2]) ^ CryptoEngineAES.gfmultby09(buffer[3, num2]));
                this.State[1, num2] = (byte)(CryptoEngineAES.gfmultby09(buffer[0, num2]) ^ CryptoEngineAES.gfmultby0e(buffer[1, num2]) ^ CryptoEngineAES.gfmultby0b(buffer[2, num2]) ^ CryptoEngineAES.gfmultby0d(buffer[3, num2]));
                this.State[2, num2] = (byte)(CryptoEngineAES.gfmultby0d(buffer[0, num2]) ^ CryptoEngineAES.gfmultby09(buffer[1, num2]) ^ CryptoEngineAES.gfmultby0e(buffer[2, num2]) ^ CryptoEngineAES.gfmultby0b(buffer[3, num2]));
                this.State[3, num2] = (byte)(CryptoEngineAES.gfmultby0b(buffer[0, num2]) ^ CryptoEngineAES.gfmultby0d(buffer[1, num2]) ^ CryptoEngineAES.gfmultby09(buffer[2, num2]) ^ CryptoEngineAES.gfmultby0e(buffer[3, num2]));
			}
		}
		private void InvShiftRows()
		{
			byte[,] buffer = new byte[4, 4];
			for (int num = 0; num < 4; num++)
			{
				for (int num2 = 0; num2 < 4; num2++)
				{
					buffer[num, num2] = this.State[num, num2];
				}
			}
			for (int num = 1; num < 4; num++)
			{
				for (int num2 = 0; num2 < 4; num2++)
				{
					this.State[num, (num2 + num) % this.Nb] = buffer[num, num2];
				}
			}
		}
		private void InvSubBytes()
		{
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					this.State[i, j] = this.iSbox[this.State[i, j] >> 4, (int)(this.State[i, j] & 15)];
				}
			}
		}
		private void KeyExpansion()
		{
			this.w = new byte[this.Nb * (this.Nr + 1), 4];
			for (int num = 0; num < this.Nk; num++)
			{
				this.w[num, 0] = this.key[4 * num];
				this.w[num, 1] = this.key[4 * num + 1];
				this.w[num, 2] = this.key[4 * num + 2];
				this.w[num, 3] = this.key[4 * num + 3];
			}
			byte[] word = new byte[4];
			for (int num = this.Nk; num < this.Nb * (this.Nr + 1); num++)
			{
				word[0] = this.w[num - 1, 0];
				word[1] = this.w[num - 1, 1];
				word[2] = this.w[num - 1, 2];
				word[3] = this.w[num - 1, 3];
				if (num % this.Nk == 0)
				{
					word = this.SubWord(this.RotWord(word));
					word[0] = (byte)(word[0] ^ this.Rcon[num / this.Nk, 0]);
					word[1] = (byte)(word[1] ^ this.Rcon[num / this.Nk, 1]);
					word[2] = (byte)(word[2] ^ this.Rcon[num / this.Nk, 2]);
					word[3] = (byte)(word[3] ^ this.Rcon[num / this.Nk, 3]);
				}
				else
				{
					if (this.Nk > 6 && num % this.Nk == 4)
					{
						word = this.SubWord(word);
					}
				}
				this.w[num, 0] = (byte)(this.w[num - this.Nk, 0] ^ word[0]);
				this.w[num, 1] = (byte)(this.w[num - this.Nk, 1] ^ word[1]);
				this.w[num, 2] = (byte)(this.w[num - this.Nk, 2] ^ word[2]);
				this.w[num, 3] = (byte)(this.w[num - this.Nk, 3] ^ word[3]);
			}
		}
		private void MixColumns()
		{
			byte[,] buffer = new byte[4, 4];
			for (int i = 0; i < 4; i++)
			{
				for (int num2 = 0; num2 < 4; num2++)
				{
					buffer[i, num2] = this.State[i, num2];
				}
			}
			for (int num2 = 0; num2 < 4; num2++)
			{
				this.State[0, num2] =(byte) (CryptoEngineAES.gfmultby02(buffer[0, num2]) ^ CryptoEngineAES.gfmultby03(buffer[1, num2]) ^ CryptoEngineAES.gfmultby01(buffer[2, num2]) ^ CryptoEngineAES.gfmultby01(buffer[3, num2]));
				this.State[1, num2] =(byte) (CryptoEngineAES.gfmultby01(buffer[0, num2]) ^ CryptoEngineAES.gfmultby02(buffer[1, num2]) ^ CryptoEngineAES.gfmultby03(buffer[2, num2]) ^ CryptoEngineAES.gfmultby01(buffer[3, num2]));
				this.State[2, num2] =(byte) (CryptoEngineAES.gfmultby01(buffer[0, num2]) ^ CryptoEngineAES.gfmultby01(buffer[1, num2]) ^ CryptoEngineAES.gfmultby02(buffer[2, num2]) ^ CryptoEngineAES.gfmultby03(buffer[3, num2]));
				this.State[3, num2] =(byte) (CryptoEngineAES.gfmultby03(buffer[0, num2]) ^ CryptoEngineAES.gfmultby01(buffer[1, num2]) ^ CryptoEngineAES.gfmultby01(buffer[2, num2]) ^ CryptoEngineAES.gfmultby02(buffer[3, num2]));
			}
		}
		private byte[] RotWord(byte[] word)
		{
			return new byte[]
			{
				word[1],
				word[2],
				word[3],
				word[0]
			};
		}
		private void SetNbNkNr(CryptoEngineAES.KeySize keySize)
		{
			this.Nb = 4;
			if (keySize == CryptoEngineAES.KeySize.Bits128)
			{
				this.Nk = 4;
				this.Nr = 10;
				return;
			}
			if (keySize == CryptoEngineAES.KeySize.Bits192)
			{
				this.Nk = 6;
				this.Nr = 12;
				return;
			}
			if (keySize == CryptoEngineAES.KeySize.Bits256)
			{
				this.Nk = 8;
				this.Nr = 14;
			}
		}
		private void ShiftRows()
		{
			byte[,] buffer = new byte[4, 4];
			for (int num = 0; num < 4; num++)
			{
				for (int num2 = 0; num2 < 4; num2++)
				{
					buffer[num, num2] = this.State[num, num2];
				}
			}
			for (int num = 1; num < 4; num++)
			{
				for (int num2 = 0; num2 < 4; num2++)
				{
					this.State[num, num2] = buffer[num, (num2 + num) % this.Nb];
				}
			}
		}
		private void SubBytes()
		{
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					this.State[i, j] = this.Sbox[this.State[i, j] >> 4, (int)(this.State[i, j] & 15)];
				}
			}
		}
		private byte[] SubWord(byte[] word)
		{
			return new byte[]
			{
				this.Sbox[word[0] >> 4, (int)(word[0] & 15)],
				this.Sbox[word[1] >> 4, (int)(word[1] & 15)],
				this.Sbox[word[2] >> 4, (int)(word[2] & 15)],
				this.Sbox[word[3] >> 4, (int)(word[3] & 15)]
			};
		}
	}
}
