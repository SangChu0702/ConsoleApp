using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class AESHelper
    {
        private static readonly byte[] key = Encoding.UTF8.GetBytes("0707200220020707"); // 16 byte key
        private static readonly byte[] iv = Encoding.UTF8.GetBytes("SangdtvddvtdgnaS");  // 16 byte IV
        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return Convert.ToBase64String(encryptedBytes);
        }
        public static string Decrypt(string encryptedText)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                var encryptedBytes = Convert.FromBase64String(encryptedText);

                var plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch
            {

            }
            return string.Empty;
        }
    }
    public class FWCrytoHelper
    {
        public static Stream Decrypt(Stream input, byte[] key)
        {
            using (var reader = new BinaryReader(input))
            {
                byte[] magicNumber = reader.ReadBytes(8);
                Console.WriteLine("Magic Number : " + Encoding.UTF8.GetString(magicNumber));

                int version = ReadInt32BigEndian(reader);
                Console.WriteLine("version : " + version);

                int signLength = ReadInt32BigEndian(reader);
                byte[] signature = reader.ReadBytes(signLength);
                Console.WriteLine("signature : " + Convert.ToBase64String(signature));

                int hashLength = ReadInt32BigEndian(reader);
                byte[] hash = reader.ReadBytes(hashLength);
                Console.WriteLine("hash : " + Convert.ToBase64String(hash));

                int _hmacLength = ReadInt32BigEndian(reader);
                byte[] _hmac = reader.ReadBytes(_hmacLength);

                byte[] iv = reader.ReadBytes(16);
                Console.WriteLine("IV : " + Convert.ToBase64String(iv));
                return Decrypt(input, key, iv);
            }
        }
        public static Stream Decrypt(Stream input, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                input.Seek(2048, SeekOrigin.Begin);
                int bytesToRead = (int)(input.Length - input.Position);
                using var reader = new BinaryReader(input);
                var encryptedBytes = reader.ReadBytes(bytesToRead);
                var plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                File.WriteAllBytes(@"C:\Users\sangdc\Downloads\SOW30050081-01_EN_decrypted.s19", plainBytes);
                return new MemoryStream(plainBytes);
            }
        }
        private static int ReadInt32BigEndian(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
    }

    public class HMACHelper
    {
        public static byte[] GetKey(byte[] seed, byte[] key)
        {
            var output = new byte[16];
            using (HMACSHA1 hmac = new HMACSHA1(key))
            {
                key = hmac.ComputeHash(seed);
                Array.Copy(key, output, 16);
            }
            return output;
        }
    }
}
