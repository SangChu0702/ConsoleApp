using System.Globalization;
using Resources.Languages;

namespace Resoucres.Helpers
{
    public class LanguageHelper
    {
        public static string GetString(string key)
        {
            return Strings.ResourceManager.GetString(key, CultureInfo.DefaultThreadCurrentUICulture) ?? string.Empty;
        }
        public static string GetString(string key, string culture)
        {
            return Strings.ResourceManager.GetString(key, CultureInfo.GetCultureInfo(culture)) ?? string.Empty;
        }
    }
}
