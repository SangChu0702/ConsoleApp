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
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }
        public static T? DeserializeObject<T>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
        public static string FormatJson(string json)
        {
            var options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            };
            var parsedJson = System.Text.Json.JsonSerializer.Deserialize<object>(json);
            return System.Text.Json.JsonSerializer.Serialize(parsedJson, options);
        }
    }
}
