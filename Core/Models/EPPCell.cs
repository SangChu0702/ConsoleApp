using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core.Models
{
    public class EPPCell
    {
        public string Name;
        public int Row;
        public int Col;
        public string[] Values;
        public EPPCell(string name, string[] values) 
        {
            Name = name;
            Values = values;
        }
        public bool IsContainValue(string? value)
        {
            if(value == null) return false;
            foreach (var item in Values) 
            {
                if (value.Contains(item.ToLower().Replace(" ", "").Replace("_", ""), StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        public bool IsHaveContent()
        {
            return Row > 0 && Col > 0;
        }
    }
}
