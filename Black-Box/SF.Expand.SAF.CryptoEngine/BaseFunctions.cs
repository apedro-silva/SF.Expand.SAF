

using System;
using System.Text;
using System.Globalization;
using System.Security.Cryptography;


namespace SF.Expand.SAF.CryptoEngine
{
    public static class BaseFunctions
    {
        /// <summary>
        /// </summary>
        /// <param name="num"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static int calcChecksum(long num, int digits)
        {
            int num2 = 0;
            bool flag = true;
            int[] dbDigit = new int[] { 0, 2, 4, 6, 8, 1, 3, 5, 7, 9 };

            while (0 < digits--)
            {
                int index = (int)(num % 10L);
                num /= 10L;
                if (flag)
                {
                    index = dbDigit[index];
                }
                num2 += index;
                flag = !flag;
            }
            int num4 = num2 % 10;
            if (num4 > 0)
            {
                num4 = 10 - num4;
            }
            return num4;
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string convertByteArrayToString(byte[] data)
        {
            int index = 0;
            char[] chArray = new char[data.Length];
            while (index < data.Length)
            {
                if (data[index] == 0)
                {
                    break;
                }
                chArray[index] = (char)data[index];
                index++;
            }
            return new string(chArray, 0, index);
        }

        /// <summary>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] convertStringToByteArray(string text)
        {
            char[] chArray = text.ToCharArray();
            byte[] buffer = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                buffer[i] = (byte)chArray[i];
            }
            return buffer;
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] HexDecoder(string data)
        {
            if ((data.Length % 2) != 0)
            {
                throw new Exception("Invalid hex value");
            }
            byte[] buffer = new byte[data.Length / 2];
            for (int i = 0; i < data.Length; i += 2)
            {
                buffer[i / 2] = byte.Parse(data.Substring(i, 2), NumberStyles.HexNumber);
            }
            return buffer;
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string HexEncoder(byte[] data)
        {
            string str = "";
            for (int i = 0; i < data.Length; i++)
            {
                str += string.Format("{0:x2}", data[i]);
            }
            return str;
        }

        /// <summary>
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] convertLongToArray(long num)
        {
            return new byte[] { ((byte)(num & 0xffL)), ((byte)((num >> 8) & 0xffL)), ((byte)((num >> 0x10) & 0xffL)), ((byte)((num >> 0x18) & 0xffL)), ((byte)((num >> 0x20) & 0xffL)), ((byte)((num >> 40) & 0xffL)), ((byte)((num >> 0x30) & 0xffL)), ((byte)((num >> 0x38) & 0xffL)) };
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte convBCD(int value)
        {
            return byte.Parse(value.ToString(), NumberStyles.HexNumber);
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String convBase(long input)
        {
            long ind1 = 0;
            long ind2 = 0;
            long _in = input;
            String _return = null;
            char[] _vetor = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'x', 'w', 'y', 'z' };

            do
            {
                ind1 = _in / (_vetor.Length - 1);
                ind2 = _in % (_vetor.Length - 1);
                _return = _vetor[(int)ind2] + _return;
                _in = ind1;
            } while (ind1 > (_vetor.Length - 1));
            return _vetor[(int)ind1] + _return;
        }

        /// <summary>
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte checkDigit(long num)
        {
            byte total = 0;
            bool doubleDigit = true;
            int digits = num.ToString().Length;
            byte[] doubleDigits = { 0, 2, 4, 6, 8, 1, 3, 5, 7, 9 };

            while (0 < digits--)
            {
                int digit = (int)(num % 10);
                num /= 10;
                if (doubleDigit)
                {
                    digit = doubleDigits[digit];
                }
                total += (byte)digit;
                doubleDigit = !doubleDigit;
            }

            byte result = (byte)(total % 10);
            if (result > 0)
            {
                result = (byte)(10 - result);
            }
            return result;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static byte CreateRandomByte()
        {
            return secureRandom(1)[0];
        }

        /// <summary>
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] secureRandom(int size)
        {
            byte[] data = new byte[size];
            new RNGCryptoServiceProvider().GetBytes(data);
            return data;
        }

        /// <summary>
        /// </summary>
        /// <param name="toEncode"></param>
        /// <returns></returns>
        public static string EncodeTo64(string toEncode)
        {
            if (toEncode == null || toEncode.Length < 1)
            {
                return toEncode;
            }
            byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="encodedData"></param>
        /// <returns></returns>
        public static string DecodeFrom64(string encodedData)
        {
            if (encodedData == null || encodedData.Length < 1)
            {
                return encodedData;
            }
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            return ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="totalSeedsLot"></param>
        /// <param name="entropy"></param>
        /// <returns></returns>
        public static string GenerateSupplierLotNumber(string totalSeedsLot, string entropy)
        {
            if (totalSeedsLot == null) return null;
            if (entropy == null) entropy = string.Empty;

            DateTime now = DateTime.Now;
            string res = now.ToString("yyddMM") + "-" + entropy.Trim() + totalSeedsLot.Trim().PadLeft(7, '0') + "-" + now.Ticks.ToString();
            return res;
        }
    }
}