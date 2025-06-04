using Core.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core.Helpers.ExcelHelper
{
    public class ODXHelper
    {
        public static List<DTCModel> GetDTCDataFromExcel(string ECU, string filePath)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Sang");
            var DTCs = new List<DTCModel>();
            using (ExcelPackage package = new ExcelPackage(filePath))
            {
                var DTCSheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name.Trim().ToUpper().Contains("DTC"));
                if (DTCSheet == null)
                {
                    Console.WriteLine("There is no DTC sheet in Excel package");
                }
                else
                {
                    var colMax = DTCSheet.Dimension.End.Row;
                    var rowMax = DTCSheet.Dimension.End.Row;
                    var startRow = 0;
                    var ListHeader = new List<EPPCell> 
                    {
                        new EPPCell( "ECUPin", new string[] {"ECU Pin", "Pin", "ECU Pin" }),
                        new EPPCell( "NumberHex",new string[] { "DTC Number (hex)" }),
                        new EPPCell( "Number", new string[] { "DTC Number" }),
                        new EPPCell( "FailureTypeByte",new string[] { "Failure Type Byte", "Failure Type Byte and meaning" }),
                        new EPPCell( "Name", new string[] { "DTC Name", "DTCName", "DTC-Name", "DTC_Name" }),
                        new EPPCell( "Priority", new string[] { "Priority" }),
                        new EPPCell( "LampFlag", new string[] { "Lamp Flag" }),
                        new EPPCell( "FailureTypeDefinition", new string[] { "DTC Failure Type Definition(if necessary)", "DTC Failure Type Definition" }),
                        new EPPCell( "RepairAction", new string[] { "Repair Action", "Repair_Action" }),
                        new EPPCell( "MonitorType", new string[] { "Monitor Type" }),
                        new EPPCell( "MonitorRate", new string[] { "Monitor Rate" }),
                        new EPPCell( "TestFailed", new string[] { "Test Failed criteria/ Confirmation Failure criteria", "Test Failed criteria" }),
                        new EPPCell( "MatureTime", new string[] { "Mature Time" }),
                        new EPPCell( "TestPass", new string[] { "Test Pass Criteria", "Test Pass" }),
                        new EPPCell( "ServiceRelevant", new string[] { "Service Relevant" }),
                        new EPPCell( "DematureTime", new string[] { "De-mature Time", "De_mature Time", "Demature Time" }),
                        new EPPCell( "FaultSymptoms", new string[] { "Fault Symptoms"}),
                        new EPPCell( "Aging", new string[] { "DTC Aging" }),
                        new EPPCell( "LocalSnapshot", new string[] { "local snapshot Record" }),
                    };
                    //Xác định các cell header
                    for (int i = 1; i < 4; i++) 
                    {
                        for (int j = 1; j <= colMax; j++) 
                        {
                            if(DTCSheet.Cells[i, j].Value == null) continue;

                            var cellValue = DTCSheet.Cells[i,j].Value.ToString()?.ToLower().Replace(" ", "").Replace("_", "");

                            if(string.IsNullOrEmpty(cellValue)) continue;

                            foreach(var item in ListHeader)
                            { 
                                if(item.IsContainValue(cellValue))
                                {
                                    item.Row = i;
                                    item.Col = j;
                                    startRow = i+1;
                                    continue;
                                }
                            }
                        }
                    }
                    foreach (var item in ListHeader)
                    {
                        if (!item.IsHaveContent())
                        {
                            Console.WriteLine($"There is no column {item.Name}");
                        }
                    }
                    //Duyệt cả sheet để lấy content

                    for (int row = startRow; row <= rowMax; row++)
                    {
                        var DTC = new DTCModel();
                        DTC.ECUName = ECU;
                        for (int col = 1; col <= colMax; col++)
                        {
                            if (DTCSheet.Cells[row, col].Value == null) continue;

                            var cellValue = DTCSheet.Cells[row, col].Value.ToString()?.Replace("\r", "").Replace("\n", "").Trim();

                            if (string.IsNullOrEmpty(cellValue)) continue;

                            foreach(var item in ListHeader)
                            {
                                if (item.Col != col) continue;
                                switch(item.Name)
                                {
                                    case "ECUPin":
                                        DTC.ECUPin = cellValue;
                                        break;
                                    case "NumberHex":
                                        DTC.DTCNumberHex = cellValue;
                                        break;
                                    case "Number":
                                        DTC.DTCNumber = cellValue;
                                        break;
                                    case "FailureTypeByte":
                                        DTC.FailureType = cellValue;
                                        break;
                                    case "Name":
                                        DTC.Name = cellValue;
                                        break;
                                    case "Priority":
                                        DTC.Priority = cellValue;
                                        break;
                                    case "LampFlag":
                                        //DTC.Lam = cellValue;
                                        break;
                                    case "FailureTypeDefinition":
                                        DTC.DTCFailureDefinition = cellValue;
                                        break;
                                    case "RepairAction":
                                        DTC.RepairAction = cellValue;
                                        break;
                                    case "MonitorType":
                                        DTC.MonitorType = cellValue;
                                        break;
                                    case "MonitorRate":
                                        DTC.MonitorRate = cellValue;
                                        break;
                                    case "TestFailed":
                                        DTC.TestFailedCriteria = cellValue;
                                        break;
                                    case "MatureTime":
                                        DTC.MatureTime = cellValue;
                                        break;
                                    case "TestPass":
                                        DTC.TestPass = cellValue;
                                        break;
                                    case "ServiceRelevant":
                                        //DTC.Ser = cellValue;
                                        break;
                                    case "DematureTime":
                                        DTC.DeMatureTime = cellValue;
                                        break;
                                    case "FaultSymptoms":
                                        DTC.FaultSymptoms = cellValue;
                                        break;
                                    case "Aging":
                                        DTC.DTCAging = cellValue;
                                        break;
                                    case "LocalSnapshot":
                                        //DTC. = cellValue;
                                        break;
                                }    
                            }
                        }
                        DTCs.Add(DTC);
                    }
                }
            }
            Console.WriteLine($"Export DTC of {ECU} => Count = {DTCs.Count}");
            return DTCs;
        }

    }
}
