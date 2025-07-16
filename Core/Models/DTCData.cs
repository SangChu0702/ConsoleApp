using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class DTCData
    {
        public string ECUPin { get; set; } = string.Empty;
        public string DTCNumberHex { get; set; } = string.Empty;

        public int DTCNum;

        public string Value = string.Empty;
        public string DTCNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FailureType { get; set; } = string.Empty;
        public string DTCFailureDefinition { get; set; } = string.Empty;
        public string RepairAction { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string MonitorType { get; set; } = string.Empty;
        public string MonitorEnableCriteria { get; set; } = string.Empty;
        public string MonitorRate { get; set; } = string.Empty;
        public string TestFailedCriteria { get; set; } = string.Empty;
        public string TestPass { get; set; } = string.Empty;
        public string ECUName { get; set; } = string.Empty;
        public string MatureTime { get; set; } = string.Empty;
        public string DeMatureTime { get; set; } = string.Empty;
        public string FaultSymptoms { get; set; } = string.Empty;

        public string Raw = string.Empty;

        public string Snapshot1904 = string.Empty;

        public string Snapshot1906 = string.Empty;

        public string DTCAging { get; set; } = string.Empty;

        public bool? Existent;

        public string Comment = string.Empty;

        public bool IsHaveSnapshot;
    }
}
