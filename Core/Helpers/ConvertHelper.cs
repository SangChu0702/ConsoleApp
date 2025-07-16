using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class ConvertHelper
    {
        public static byte[] HexToBytes(string hex)
        {
            int len = hex.Length;
            if (len % 2 != 0)
                throw new ArgumentException("Hex string must have even length.");
            byte[] data = new byte[len / 2];

            for (int i = 0; i < len; i += 2)
            {
                data[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return data;
        }
        public static string BytesToHex(byte[] bytes, bool upperCase = true)
        {
            var hex = BitConverter.ToString(bytes).Replace("-", "");
            return upperCase ? hex.ToUpperInvariant() : hex.ToLowerInvariant();
        }
    }
    public class ByteHelper
    {
        public static byte[] TrimPadding(byte[] input, byte[] paddingValues)
        {
            int start = 0;
            int end = input.Length - 1;

            while (start <= end && paddingValues.Contains(input[start]))
                start++;

            while (end >= start && paddingValues.Contains(input[end]))
                end--;

            int length = end - start + 1;
            if (length <= 0)
                return Array.Empty<byte>();

            return input.Skip(start).Take(length).ToArray();
        }
    }


}
