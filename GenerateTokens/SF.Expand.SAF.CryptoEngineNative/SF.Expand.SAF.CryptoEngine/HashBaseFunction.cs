using System;
namespace SF.Expand.SAF.CryptoEngine
{
	public static class HashBaseFunction
	{
		private const uint _H00 = 1779033703u;
		private const uint _H10 = 3144134277u;
		private const uint _H20 = 1013904242u;
		private const uint _H30 = 2773480762u;
		private const uint _H40 = 1359893119u;
		private const uint _H50 = 2600822924u;
		private const uint _H60 = 528734635u;
		private const uint _H70 = 1541459225u;
		public static byte[] createBinaryHash(string data)
		{
			return HashBaseFunction.createBinaryHash(BaseFunctions.convertStringToByteArray(data));
		}
		public static byte[] createBinaryHash(byte[] message)
		{
			int i = message.Length >> 6;
			int num2;
			if ((message.Length & 63) < 56)
			{
				i++;
				num2 = 56;
			}
			else
			{
				i += 2;
				num2 = 120;
			}
			long num3 = (long)((long)message.Length << 3);
			byte[] sourceArray = new byte[num2 - (message.Length & 63)];
			sourceArray[0] = 128;
			for (int j = 1; j < sourceArray.Length; j++)
			{
				sourceArray[j] = 0;
			}
			byte[] destinationArray = new byte[i * 64];
			Array.Copy(message, 0, destinationArray, 0, message.Length);
			Array.Copy(sourceArray, 0, destinationArray, message.Length, sourceArray.Length);
			byte[] bytes = BitConverter.GetBytes(num3);
			byte[] buffer3 = new byte[8];
			for (int k = 0; k < 8; k++)
			{
				buffer3[k] = bytes[7 - k];
			}
			Array.Copy(buffer3, 0, destinationArray, message.Length + sourceArray.Length, 8);
			return HashBaseFunction._Hash256Bits(i, destinationArray);
		}
		public static string createStringHash(byte[] data)
		{
			return BaseFunctions.convertByteArrayToString(HashBaseFunction.createBinaryHash(data));
		}
		public static string createStringHash(string data)
		{
			return BaseFunctions.convertByteArrayToString(HashBaseFunction.createBinaryHash(BaseFunctions.convertStringToByteArray(data)));
		}
		private static uint Func(bool b, uint x, uint y, uint z)
		{
			if (b)
			{
				return (x & y) ^ (~x & z);
			}
			return (x & y) ^ (x & z) ^ (y & z);
		}
		private static byte[] _Hash256Bits(int n, byte[] m)
		{
			uint[] numArray = new uint[]
			{
				1116352408u,
				1899447441u,
				3049323471u,
				3921009573u,
				961987163u,
				1508970993u,
				2453635748u,
				2870763221u,
				3624381080u,
				310598401u,
				607225278u,
				1426881987u,
				1925078388u,
				2162078206u,
				2614888103u,
				3248222580u,
				3835390401u,
				4022224774u,
				264347078u,
				604807628u,
				770255983u,
				1249150122u,
				1555081692u,
				1996064986u,
				2554220882u,
				2821834349u,
				2952996808u,
				3210313671u,
				3336571891u,
				3584528711u,
				113926993u,
				338241895u,
				666307205u,
				773529912u,
				1294757372u,
				1396182291u,
				1695183700u,
				1986661051u,
				2177026350u,
				2456956037u,
				2730485921u,
				2820302411u,
				3259730800u,
				3345764771u,
				3516065817u,
				3600352804u,
				4094571909u,
				275423344u,
				430227734u,
				506948616u,
				659060556u,
				883997877u,
				958139571u,
				1322822218u,
				1537002063u,
				1747873779u,
				1955562222u,
				2024104815u,
				2227730452u,
				2361852424u,
				2428436474u,
				2756734187u,
				3204031479u,
				3329325298u
			};
			uint[] numArray2 = new uint[8];
			uint[] numArray3 = new uint[64];
			byte[] destinationArray = new byte[32];
			byte[] buffer2 = new byte[4];
			byte[] bytes = new byte[4];
			numArray2[0] = 1779033703u;
			numArray2[1] = 3144134277u;
			numArray2[2] = 1013904242u;
			numArray2[3] = 2773480762u;
			numArray2[4] = 1359893119u;
			numArray2[5] = 2600822924u;
			numArray2[6] = 528734635u;
			numArray2[7] = 1541459225u;
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < 64; j++)
				{
					if (j < 16)
					{
						for (int num13 = 0; num13 < 4; num13++)
						{
							buffer2[num13] = m[i * 64 + j * 4 + 3 - num13];
						}
						numArray3[j] = BitConverter.ToUInt32(buffer2, 0);
					}
					else
					{
						numArray3[j] = HashBaseFunction._sigmaFunct(3, numArray3[j - 2]) + numArray3[j - 7] + HashBaseFunction._sigmaFunct(2, numArray3[j - 15]) + numArray3[j - 16];
					}
				}
				uint x = numArray2[0];
				uint y = numArray2[1];
				uint z = numArray2[2];
				uint num14 = numArray2[3];
				uint num15 = numArray2[4];
				uint num16 = numArray2[5];
				uint num17 = numArray2[6];
				uint num18 = numArray2[7];
				for (int num19 = 0; num19 < 64; num19++)
				{
					uint num20 = num18 + HashBaseFunction._sigmaFunct(1, num15) + HashBaseFunction.Func(true, num15, num16, num17) + numArray[num19] + numArray3[num19];
					uint num21 = HashBaseFunction._sigmaFunct(0, x) + HashBaseFunction.Func(false, x, y, z);
					num18 = num17;
					num17 = num16;
					num16 = num15;
					num15 = num14 + num20;
					num14 = z;
					z = y;
					y = x;
					x = num20 + num21;
				}
				numArray2[0] = x + numArray2[0];
				numArray2[1] = y + numArray2[1];
				numArray2[2] = z + numArray2[2];
				numArray2[3] = num14 + numArray2[3];
				numArray2[4] = num15 + numArray2[4];
				numArray2[5] = num16 + numArray2[5];
				numArray2[6] = num17 + numArray2[6];
				numArray2[7] = num18 + numArray2[7];
			}
			for (int k = 0; k < 8; k++)
			{
				bytes = BitConverter.GetBytes(numArray2[k]);
				for (int num22 = 0; num22 < 4; num22++)
				{
					buffer2[num22] = bytes[3 - num22];
				}
				Array.Copy(buffer2, 0, destinationArray, k * 4, 4);
			}
			return destinationArray;
		}
		private static uint _sigmaFunct(int i, uint x)
		{
			switch (i)
			{
			case 0:
			{
				uint num = x >> 2 | x << 30;
				num ^= (x >> 13 | x << 19);
				return num ^ (x >> 22 | x << 10);
			}
			case 1:
			{
				uint num = x >> 6 | x << 26;
				num ^= (x >> 11 | x << 21);
				return num ^ (x >> 25 | x << 7);
			}
			case 2:
			{
				uint num = x >> 7 | x << 25;
				num ^= (x >> 18 | x << 14);
				return num ^ x >> 3;
			}
			case 3:
			{
				uint num = x >> 17 | x << 15;
				num ^= (x >> 19 | x << 13);
				return num ^ x >> 10;
			}
			default:
				return x;
			}
		}
	}
}
