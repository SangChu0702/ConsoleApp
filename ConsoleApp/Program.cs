using Core.Helpers.ExcelHelper;
using Core.Models;
using OfficeOpenXml.Style;

Console.WriteLine("Please enter excel path:");
var folderPath = Console.ReadLine();
if(!string.IsNullOrEmpty(folderPath))
{
    var Model = Path.GetDirectoryName(folderPath);
    var result = new Dictionary<string, List<DTCModel>>();
    var direcs = Directory.GetDirectories(folderPath);
    foreach(string folder in direcs)
    {
        var ECUName = Path.GetDirectoryName(folder);

        if(string.IsNullOrEmpty(ECUName)) continue;

        var excelPath = Directory.GetFiles(folder).FirstOrDefault();

        if(excelPath != null)
        {
            var ecuDTCs = ODXHelper.GetDTCDataFromExcel(ECUName, excelPath);
            result.Add(ECUName, ecuDTCs);
        }
    }
    Console.ReadLine();
}
