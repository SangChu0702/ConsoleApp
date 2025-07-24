using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Models;
using OfficeOpenXml;

namespace Core.Helpers.Excel
{
    public class ExcelHelper
    {
        public static Dictionary<string, VehicleInfo> ConvertToVehInfoDB(string filePath)
        {
            var CarDB = new Dictionary<string, VehicleInfo>();
            ExcelPackage.License.SetNonCommercialPersonal("Sang");
            using (ExcelPackage PKG = new ExcelPackage(filePath))
            {
                foreach(var sheet in PKG.Workbook.Worksheets)
                {
                    string key = sheet.Name;
                    Console.WriteLine(key);
                    var VehInfo = new VehicleInfo();
                    //Khai báo list Header
                    var ListHeader = new List<EPPCell>
                    {
                        new EPPCell( "Model", new string[] {"Model", "MODEL" }),
                        new EPPCell( "ModelCode",new string[] { "ModelCode", "Model Code", "MODEL CODE" }),
                        new EPPCell( "EcuID", new string[] { "EcuID", "ECU ID" }),
                        new EPPCell( "EcuName",new string[] { "ECU Name", "ECU NAME" }),
                        new EPPCell( "Optional", new string[] { "Optional", "OPTIONAL"}),
                        new EPPCell( "FullName", new string[] { "FullName", "FULL NAME", "Full Name" }),
                        new EPPCell( "ListNames", new string[] { "ListNames", "LISTNAMES" }),
                        new EPPCell( "DiagLocation", new string[] { "DIAG LOCATION", "Diag Location" }),
                        new EPPCell( "CustomFlash", new string[] { "CustomFlash", "CUSTOMFLASH", "Customflash?" }),
                        new EPPCell( "FlashDLL", new string[] { "FlashDLL" }),
                        new EPPCell( "DivaDLL", new string[] { "DivaDLL" }),
                        new EPPCell( "DIDs", new string[] { "DIDs" }),
                        new EPPCell( "Database", new string[] { "Database", "DatabaseKey" }),
                    };
                    int rowMax = sheet.Dimension.End.Row;
                    int startRow = 0;
                    int colMax = 0;
                    //Lấy vị trí các Header
                    for (int i = 1; i < 5; i++)
                    {
                        for (int j = 1; j <= 20; j++)
                        {
                            if (sheet.Cells[i, j].Value == null) continue;
                            var cellValue = StringHelper.RemoveNonLetterChar(sheet.Cells[i, j].Value.ToString() ?? "");
                            if (string.IsNullOrEmpty(cellValue)) continue;

                            foreach (var header in ListHeader)
                            {
                                if (header.IsContainValue(cellValue) && header.Col == 0)
                                {
                                    header.Row = i;
                                    header.Col = header.Col != 0 ? header.Col : j;
                                    startRow = i + 1;
                                    colMax = colMax < j ? j : colMax;
                                    break;
                                }
                            }
                        }
                        if (startRow > i) break;
                    }
                    var ModelCodeCol = ListHeader.First(x => x.Name == "ModelCode").Col;
                    if (ModelCodeCol != 0)
                    {
                        //Lấy value
                        for (int r = startRow; r <= rowMax; r++)
                        {
                            var EcuInfo = new ECUInfo();
                            for (int c = 1; c <= colMax; c++)
                            {
                                if (sheet.Cells[r, c].Value == null) continue;
                                var cellValue = sheet.Cells[r, c].Value.ToString();
                                if (string.IsNullOrEmpty(cellValue)) continue;

                                foreach (var header in ListHeader)
                                {
                                    if (header.Col == c)
                                    {
                                        switch (header.Name)
                                        {
                                            case "Model":
                                                if (string.IsNullOrEmpty(VehInfo.Model)) VehInfo.Model = cellValue;
                                                break;
                                            case "Database":
                                                if (string.IsNullOrEmpty(VehInfo.DataKey)) VehInfo.DataKey = cellValue;
                                                break;
                                            case "ModelCode":
                                                VehInfo.ModelCodes.Add(cellValue);
                                                break;
                                            case "EcuID":
                                                EcuInfo.ID = cellValue;
                                                break;
                                            case "EcuName":
                                                EcuInfo.Name = cellValue;
                                                break;
                                            case "Optional":
                                                EcuInfo.IsOptional = cellValue.Equals("x", StringComparison.OrdinalIgnoreCase);
                                                break;
                                            case "FullName":
                                                EcuInfo.FullName = cellValue;
                                                break;
                                            case "ListNames":
                                                var parts = cellValue.Split(',');
                                                foreach (var part in parts)
                                                    EcuInfo.ListNames.Add(part.Trim());
                                                break;
                                            case "DiagLocation":
                                                EcuInfo.Location = cellValue;
                                                break;
                                            case "CustomFlash":
                                                EcuInfo.IsCustomFlash = cellValue == "x";
                                                break;
                                            case "FlashDLL":
                                                EcuInfo.FlashDLL = cellValue;
                                                break;
                                            case "DivaDLL":
                                                EcuInfo.DivaDLL = cellValue;
                                                break;
                                            case "DIDs":
                                                var matches = Regex.Matches(cellValue, @"""(\w+)"":\s*\[\s*([^]]+)\s*\]");
                                                foreach (Match match in matches)
                                                {
                                                    string DIDkey = match.Groups[1].Value;
                                                    string valuesRaw = match.Groups[2].Value;

                                                    // Tách từng giá trị, loại bỏ dấu nháy và khoảng trắng
                                                    var values = Regex.Matches(valuesRaw, @"""([^""]+)""")
                                                                      .Select(m => m.Groups[1].Value)
                                                                      .ToList();

                                                    EcuInfo.DIDS[DIDkey] = values;
                                                }
                                                break;
                                        }
                                        break;
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(EcuInfo.ID) && string.IsNullOrEmpty(sheet.Cells[r, ModelCodeCol].Value?.ToString() ?? ""))
                                break;
                            VehInfo.AllECUs.Add(EcuInfo);
                        }
                        //Add vào DB
                        if(!string.IsNullOrEmpty(VehInfo.DataKey))
                            CarDB[key] = VehInfo;
                    }
                    else Console.Write(" => Not contain Model Code");
                }
            }
            return CarDB;
        }
    }
}
