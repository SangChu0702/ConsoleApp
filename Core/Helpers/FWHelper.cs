using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.FirmwareFile;
using Force.Crc32;

namespace Core.Helpers
{
    public class FWHelper
    {
        public string GetCRC32(Firmware fw)
        {
            int totalLength = fw.Blocks.Sum(b => b.Data.Length);
            byte[] data = new byte[totalLength];
            int offset = 0;
            foreach (var block in fw.Blocks)
            {
                Buffer.BlockCopy(block.Data, 0, data, offset, block.Data.Length);
                offset += block.Data.Length;
            }
            uint crc = Crc32Algorithm.Compute(data);
            return crc.ToString("X8").ToUpperInvariant();
        }
        public Firmware LoadFW(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            switch (extension)
            {
                case ".hex":
                    return IntelFileLoader.Load(filePath);
                case ".srec":
                case ".s19":
                case ".s28":
                case ".s37":
                case ".sre":
                case ".sx":
                    return MotorolaFileLoader.Load(filePath);

                default:
                    return BinaryFileLoader.Load(filePath);
            }
        }
    }
}
