using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class JsonHelper
    {
        public static string SerializeObject<T>(T obj)
        {
            var options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            };
            return System.Text.Json.JsonSerializer.Serialize(obj, options);
        }
        public static T? DeserializeObject<T>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
    }
}
