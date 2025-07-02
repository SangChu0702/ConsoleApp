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
}
