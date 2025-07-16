using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Resoucres.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool CollapseInsteadOfHide { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = value is bool b && b;

            // Nếu parameter là "!", thì đảo giá trị
            if (parameter is string param && param.Equals("!", StringComparison.OrdinalIgnoreCase))
                flag = !flag;

            if (flag)
                return Visibility.Visible;

            return CollapseInsteadOfHide ? Visibility.Collapsed : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool result = visibility == Visibility.Visible;

                if (parameter is string param && param.Equals("!", StringComparison.OrdinalIgnoreCase))
                    result = !result;

                return result;
            }

            return false;
        }
    }
}
