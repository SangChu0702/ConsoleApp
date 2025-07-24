using System.Management;
using System.Threading.Tasks;
using Core;
using Core.Helpers;
using Core.Helpers.Excel;
using Core.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        LogHelper.Init(Utils.OutputLogPath);
        ExportCarDB();
        LogHelper.Dispose();
    }
    static void ExportDTCJson()
    {
        Console.WriteLine("Please enter excel path:");
        var folderPath = Console.ReadLine();
        Console.WriteLine("Please enter save path:");
        var savePath = Console.ReadLine();
        if (!string.IsNullOrEmpty(folderPath))
        {
            var Model = Path.GetDirectoryName(folderPath);
            var result = new Dictionary<string, List<DTCData>>();

            var direcs = Directory.GetDirectories(folderPath.Replace("\"", ""));
            foreach (string folder in direcs)
            {
                var ECUName = Path.GetFileNameWithoutExtension(folder);

                if (string.IsNullOrEmpty(ECUName)) continue;

                var excelPath = Directory.GetFiles(folder).FirstOrDefault();

                if (excelPath != null /*&& (ECUName == "POD_GW" || ECUName == "PAS_LITE")*/)
                {
                    var ecuDTCs = ODXHelper.GetDTCDataFromExcel(ECUName, excelPath);
                    Console.WriteLine($"Export DTC of {ECUName} => Count = {ecuDTCs.Count}");
                    result.Add(ECUName, ecuDTCs);
                }
            }
            if (result.Count > 0)
            {
                try
                {
                    var save = savePath + "\\DTC.json";
                    var json = JsonHelper.SerializeObject(result);
                    Console.WriteLine($"Saved");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing files: {ex.Message}");
                    return;
                }
            }
            Console.ReadLine();
        }
    }
    static void HackKey()
    {
        object locker = new();
        Console.WriteLine("Enter Seed:");
        var SeedInput = Console.ReadLine()?.Replace(" ", "");
        Console.WriteLine("Enter Key:");
        var KeyOutput = Console.ReadLine()?.Replace(" ", "");
        var Key = new byte[] { 0x6A, 0x6A, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00 };
        Parallel.For(0, 256, (i, stateOuter) =>
        {
            for (int j = 0; j < 256; j++)
            {
                for (int k = 0; k < 256; k++)
                {
                    for (int l = 0; l < 256; l++)
                    {
                        for (int m = 0; m < 256; l++)
                        {
                            var trialKey = new byte[8];
                            Buffer.BlockCopy(Key, 0, trialKey, 0, 4);
                            trialKey[3] = (byte)i;
                            trialKey[4] = (byte)j;
                            trialKey[5] = (byte)k;
                            trialKey[6] = (byte)l;
                            trialKey[7] = (byte)m;
                            var seedBytes = ConvertHelper.HexToBytes(SeedInput ?? string.Empty);
                            var result = HMACHelper.GetKey(seedBytes, trialKey);
                            var resultHex = ConvertHelper.BytesToHex(result);

                            if (resultHex.Equals(KeyOutput, StringComparison.OrdinalIgnoreCase))
                            {
                                lock (locker)
                                {
                                    Console.WriteLine("✔ Found key: " + ConvertHelper.BytesToHex(trialKey));
                                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.txt"), ConvertHelper.BytesToHex(trialKey));
                                    stateOuter.Stop();
                                }
                            }
                        }
                    }
                }
            }
        });
        Console.WriteLine("✔ Run full ");
        Console.ReadLine();
    }
    static void ExportCarDB()
    {
        string? excelPath = Console.ReadLine();
        if(excelPath != null && File.Exists(excelPath))
        {
            var saveFolder = Path.GetDirectoryName(excelPath);
            if(saveFolder != null)
            {
                var CarDB = ExcelHelper.ConvertToVehInfoDB(excelPath);
                if (CarDB.Count > 0)
                {
                    string content = JsonHelper.SerializeObject<Dictionary<string, VehicleInfo>>(CarDB);
                    File.WriteAllText(Path.Combine(saveFolder, "AllEcuDid.json"), content);
                }
            }
        }
    }
    static void TestDecrypt()
    {
        Console.WriteLine("Input SW Path");
        var swPath = Console.ReadLine();
        if (!string.IsNullOrEmpty(swPath) && File.Exists(swPath))
        {
            string key = "f3f793219cfc0de671e1281c97bc9cca4d522733da29b6f5e143858a77553d76";
            var keyBytes = ConvertHelper.HexToBytes(key);
            FWCrytoHelper.Decrypt(File.OpenRead(swPath), keyBytes);
        }
    }
    static void TestEncrypt()
    {
        while (true)
        {
            Console.WriteLine("Input SW Folder");
            var folder = Console.ReadLine();
            if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
            {
                string saveFolder = Path.Combine(folder, "EN");
                if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

                var listFile = Directory.GetFiles(folder);
                foreach (var file in listFile)
                {
                    if (Path.GetFileName(file).Contains("EN")) continue;
                    var fileName = Path.GetFileName(file).Replace("UN", "EN");
                    List<byte> bytes = new List<byte>();
                    bytes.AddRange(ConvertHelper.HexToBytes("0056465F464F5441000000010000010046D94A825E5CE6031F1E611595037456E3B6FBFFFAD65154E2261A920E4F9338B98D4DC7A15D8A5DA30697C87017FA322C76710591211365E60BBFEBBEC9D25C11FF28042529EE1B4F347D2A41127F31758F98FF26F40E44E6BA079E05F8763E72BD22D29F02669917AC8CA12D0539D72D10241944F103AB818502F2A9A9E5A6E61E83EBD4071280BF8EA16E467B47B46ECD4B0E4555CF8C125D3352B8C72D43EF9BFF4CE8F7B8D71E8722C786CBCBF640D6ECA4272B30D553352449ABD8BB0E9B1EA54EE7E18DFD73ECFA8E0C8F598D6F9B964118A8910E504B7E876C90079A35002DD7AA20EB2DCC18D6FD706AB4624287F9B797E1BF67D987ECBB224A756600000020A1EF58854207E733227D2CAEF1739AE344757E2840B2B3FEB0D024685F4270F9000000201FF4B930744F3C59F28BCC3DD8F3DBB2C2DA88F3209964D8BEF8BF673157824C06014D6FE4A6AAE5BA5E4D1A70C648AB"));
                    bytes.AddRange(new byte[1688]);
                    bytes.AddRange(FWCrytoHelper.Encrypt(File.OpenRead(file)));
                    File.WriteAllBytes(Path.Combine(saveFolder, fileName), bytes.ToArray());
                    Console.WriteLine("Encrypt OK");
                }
            }
        }
    }
}