using System.Net.WebSockets;
using Core.Helpers;
using Core.Helpers.ExcelHelper;
using Core.Models;
using OfficeOpenXml.Style;

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
