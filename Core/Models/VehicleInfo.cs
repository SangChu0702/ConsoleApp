using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class VehicleInfo
    {
        public string Model { get; set; } = string.Empty;
        public string DataKey { get; set; } = string.Empty;
        public List<string> ModelCodes { get; set; } = new ();
        public List<ECUInfo> AllECUs { get; set; } = new ();
    }
    public class ECUInfo
    {
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> ListNames { get; set; } = new();
        public string ID { get; set; } = string.Empty;
        public string FlashDLL { get; set; } = string.Empty;
        public string DivaDLL { get; set; } = string.Empty;
        public bool EnableExtend { get; set; }
        public bool IsOptional { get; set; }
        public bool IsCustomFlash { get; set; }
        public Dictionary<string, List<string>> DIDS { get; set; } = new ();
        public string Location { get; set; } = string.Empty;
        public int FaultCount { get; set; }
        public ECUStatus Status { get; set; }
    }
    
}
