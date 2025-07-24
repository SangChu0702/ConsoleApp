using Core.Models;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core.Helpers.Excel
{
    public class ODXHelper
    {
        public static Dictionary<string, string> TypeBytes = new Dictionary<string, string>
        {
            { "85", "Signal Above Allowable Range" },
            { "84", "Signal Below Allowable Range" },
            { "81", "Invalid Serial Data Received" },
            { "96", "Component Internal Failure" },
            { "9E", "Stuck ON" },
            { "87", "Missing Message" },
            { "98", "Component or System Over Temperature" },
            { "17", "Circuit Voltage Above Threshold" },
            { "16", "Circuit Voltage Below Threshold" },
            { "1F", "Circuit Intermittent" },
            { "12", "Circuit Short To Battery" },
            { "11", "Circuit Short To Ground" },
            { "1C", "Circuit Voltage Out of Range" },
            { "64", "Signal Plausibility Failure" },
            { "97", "Component or System Operation Obstructed or Blocked" },
            { "54", "Missing Calibration" },
            { "01", "General Circuit/Electrical Failure" },
            { "07", "Mechanical Failure" },
            { "68", "Event Information" },
            { "71", "Actuator Stuck" },
            { "62", "Signal Compare Failure" },
            { "00", "No Sub Type Information" },
            { "08", "Bus Signal / Message Failure" },
            { "18", "Circuit Current Below Threshold" },
            { "73", "Actuator Stuck Closed" },
            { "72", "Actuator Stuck Open" },
            { "13", "Circuit Open" },
            { "03", "FM (Frequency Modulated) / PWM (Pulse Width Modulated) Failure" },
            { "04", "System Internal Failures" },
            { "29", "Signal Invalid" },
            { "1D", "Circuit Current Out of Range" },
            { "49", "Internal Electronic Failure" },
            { "94", "No Operation" },
            { "38", "Signal Frequency Incorrect" },
            { "47", "Watchdog / Safety MicroController failure" },
            { "25", "Signal Shape / Waveform Failure" },
            { "55", "Not Configuration" },
            { "56", "Invalid / Incompatible Configuration" },
            { "88", "Bus Off" },
            { "95", "Incorrect Assembly" },
            { "92", "Performance or Incorrect Operation" },
            { "9A", "Component or System Operating Conditions" },
            { "99", "Exceeded Learning Limit" },
            { "19", "Circuit Current Above Threshold" },
            { "09", "Component Failure" },
            { "82", "Alive/Sequence Counter Incorrect" },
            { "83", "Value of Signal Protection Calculation Incorrect" },
            { "42", "General Memory Failure" },
            { "44", "Memory Failure" },
            { "45", "Program Memory Failure" },
            { "10", "No Sub Type Information" },
            { "14", "No Sub Type Information" },
            { "15", "No Sub Type Information" },
            { "20", "No Sub Type Information" },
            { "21", "No Sub Type Information" },
            { "22", "No Sub Type Information" },
            { "23", "No Sub Type Information" },
            { "24", "No Sub Type Information" },
            { "26", "No Sub Type Information" },
            { "27", "No Sub Type Information" },
            { "28", "No Sub Type Information" },
            { "30", "No Sub Type Information" }

        };
        public static List<DTCData> GetDTCDataFromExcel(string ECU, string filePath)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Sang");
            var DTCs = new List<DTCData>();
            using (ExcelPackage package = new ExcelPackage(filePath))
            {
                var DTCSheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name.Trim().ToUpper().Contains("DTC") && x.Name.Trim().ToUpper().Contains("LIST"));
                if (DTCSheet == null)
                {
                    Console.WriteLine("There is no DTC sheet in Excel package");
                }
                else
                {
                    var rowMax = DTCSheet.Dimension.End.Row;
                    var startRow = 0;
                    var ListHeader = new List<EPPCell>
                    {
                        new EPPCell( "ECUPin", new string[] {"ECU Pin", "Pin", "ECU Pin" }),
                        new EPPCell( "NumberHex",new string[] { "DTC Number (hex)", "DTC Number Hex" }),
                        new EPPCell( "Number", new string[] { "DTC Number", "DTC Display Number" }),
                        new EPPCell( "FailureTypeByte",new string[] { "Failure Type Byte", "Failure Type Byte and meaning", "FTB (hex)" }),
                        new EPPCell( "Name", new string[] { "DTC Name", "DTCName", "DTC-Name", "DTC_Name" ,"SOC initialization error of Pack"}),
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
                    startRow = 999;
                    int start = ECU.Contains("EAC") ? 2 : 1;
                    for (int i = start; i < startRow - 1; i++)
                    {
                        for (int j = 1; j <= 30; j++)
                        {
                            try
                            {
                                if (DTCSheet.Cells[i, j].Value == null) continue;
                                var cellValue = DTCSheet.Cells[i, j].Value.ToString()?.ToLower().Replace(" ", "").Replace("_", "").Replace("-", "").Replace("\n", "").Replace("\r", "");
                                if (string.IsNullOrEmpty(cellValue)) continue;
                                foreach (var item in ListHeader)
                                {
                                    if (item.IsContainValue(cellValue))
                                    {
                                        item.Row = i;
                                        item.Col = item.Col != 0 ? item.Col : j;
                                        startRow = i + 1;
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                                break;
                            }
                        }
                    }
                    //Duyệt cả sheet để lấy content
                    bool isEnd = false;
                    for (int row = startRow; row <= rowMax; row++)
                    {
                        var DTC = new DTCData();
                        DTC.ECUName = ECU;
                        for (int col = 1; col <= 30; col++)
                        {
                            try
                            {
                                if (DTCSheet.Cells[row, col].Value == null) continue;

                                var cellValue = DTCSheet.Cells[row, col].Value.ToString()?.Replace("\r", "").Replace("\n", "").Trim();

                                if (string.IsNullOrEmpty(cellValue)) continue;
                                foreach (var item in ListHeader)
                                {
                                    if (item.Col == col)
                                    {
                                        switch (item.Name)
                                        {
                                            case "ECUPin":
                                                DTC.ECUPin = cellValue;
                                                break;
                                            case "NumberHex":
                                                DTC.DTCNumberHex = cellValue.Replace("0x", "").Replace("0X", "");
                                                break;
                                            case "Number":
                                                DTC.DTCNumber = FormatDtcNumber(cellValue);
                                                break;
                                            case "FailureTypeByte":
                                                DTC.FailureType = FormatFailureTypeByte(cellValue);
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
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                                isEnd = true;
                                break;
                            }
                        }
                        if (isEnd) break;
                        //For VFe34
                        if (ECU == "VCU" && !string.IsNullOrEmpty(DTC.DTCNumber))
                        {
                            DTC.DTCNumber = FormatDtcNumberForVCU(DTC.DTCNumber, DTC.FailureType.Substring(0, 2));
                            DTC.DTCNumberHex = DtcNumberToDtcNumberHex(DTC.DTCNumber);
                        }
                        //if (DTC.DTCNumberHex.Trim().Length < 6) DTC.DTCNumberHex = DtcNumberToDtcNumberHex(DTC.DTCNumber);
                        if (!string.IsNullOrEmpty(DTC.DTCNumber) && !string.IsNullOrEmpty(DTC.Name) && !string.IsNullOrEmpty(DTC.DTCNumberHex)) DTCs.Add(DTC);
                    }
                }
            }
            return DTCs;
        }
        private static string FormatDtcNumber(string input)
        {
            var formated = input.Replace(" ", "").Replace("-", "").Replace("_", "").ToUpperInvariant();
            if (formated.Length == 7)
            {
                return formated.Substring(0, 5) + "-" + formated.Substring(5);
            }
            return formated;
        }
        private static string FormatFailureTypeByte(string input)
        {
            var basic = input.Replace("0x", "").Replace("_", "-");
            var parts = basic.Split('-');
            if (parts.Length == 2)
            {
                return parts[0].Trim() + " - " + parts[1].Trim();
            }
            else if (parts.Length == 1)
            {
                if (TypeBytes.ContainsKey(parts[0]))
                {
                    return parts[0].Trim() + " - " + TypeBytes[parts[0]];
                }
                else if (basic.Contains("n.a"))
                {
                    return input.Substring(0, 2).Trim() + " - " + input.Substring(2).Trim();
                }
            }
            return basic;
        }
        private static string FormatDtcNumberForVCU(string input, string failureByte)
        {
            if (input.Length < 7)
            {
                return input + "-" + failureByte;
            }
            return input;
        }
        private static string DtcNumberToDtcNumberHex(string input)
        {
            input = input.Replace("-", "");
            if (input.Length != 7) return input;
            byte letter = (byte)(input[0] switch
            {
                'P' => 0b00,
                'C' => 0b01,
                'B' => 0b10,
                'U' => 0b11,
                _ => throw new ArgumentException("Invalid letter")
            } << 6 | (byte)input[1] << 4);
            string fisrt = letter.ToString("X").Replace("-", "");
            return fisrt[0] + input.Substring(2);
        }
    }
}
