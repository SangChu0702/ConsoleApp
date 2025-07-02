using System.Diagnostics.Metrics;
using System.Net.WebSockets;
using Core.Helpers;
using Core.Helpers.ExcelHelper;
using Core.Models;
using OfficeOpenXml.Style;


HackKey();

static void ExportDTCJson()
{
    Console.WriteLine("Please enter excel path:");
    var folderPath = Console.ReadLine();
    Console.WriteLine("Please enter save path:");
    var savePath = Console.ReadLine();
    if (!string.IsNullOrEmpty(folderPath))
    {
        var Model = Path.GetDirectoryName(folderPath);
        var result = new Dictionary<string, List<DTCModel>>();

        var direcs = Directory.GetDirectories(folderPath.Replace("\"", ""));
        foreach (string folder in direcs)
        {
            var ECUName = Path.GetFileNameWithoutExtension(folder);

            if (string.IsNullOrEmpty(ECUName)) continue;

            var excelPath = Directory.GetFiles(folder).FirstOrDefault();

            if (excelPath != null)
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
                File.WriteAllText(save, JsonHelper.FormatJson(json));
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
    var Key = new byte[] { 0x6A, 0x6A, 0xAA, 0xA3, 0x00 , 0x00, 0x00, 0x00 };
    Parallel.For(0, 256, (i, stateOuter) =>
    {
        for (int j = 0; j < 256; j++)
        {
            for (int k = 0; k < 256; k++)
            {
                for (int l = 0; l < 256; l++)
                {
                    var trialKey = new byte[8];
                    Buffer.BlockCopy(Key, 0, trialKey, 0, 4);
                    trialKey[4] = (byte)i;
                    trialKey[5] = (byte)j;
                    trialKey[6] = (byte)k;
                    trialKey[7] = (byte)l;
                    var seedBytes = ConvertHelper.HexToBytes(SeedInput ?? string.Empty);
                    var result = HMACHelper.GetKey(seedBytes, trialKey);
                    var resultHex = ConvertHelper.BytesToHex(result);

                    if (resultHex.Equals(KeyOutput, StringComparison.OrdinalIgnoreCase))
                    {
                        lock (locker)
                        {
                            Console.WriteLine("✔ Found key: " + ConvertHelper.BytesToHex(trialKey));
                            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.txt") , ConvertHelper.BytesToHex(trialKey));
                            stateOuter.Stop();
                        }
                    }
                }
            }
        }
    });
    Console.ReadLine();
}
