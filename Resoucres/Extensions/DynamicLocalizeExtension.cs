using Resoucres.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Resoucres.Extensions
{
    public class DynamicLocalizeExtension : MarkupExtension, INotifyPropertyChanged
    {
        public string Key { get; set; } = string.Empty;
        public object[] Parameters { get; set; } = { };

        public DynamicLocalizeExtension()
        {
            // Subscribe to language changes
            LocalizationService.Instance.PropertyChanged += OnLanguageChanged;
        }

        public DynamicLocalizeExtension(string key) : this()
        {
            Key = key;
        }

        public string Value
        {
            get
            {
                if (string.IsNullOrEmpty(Key))
                    return "[NO KEY]";

                try
                {
                    var format = Resources.Languages.Strings.ResourceManager.GetString(Key, LocalizationService.Instance.CurrentCulture);
                    if (string.IsNullOrEmpty(format))
                        return $"[{Key}]";

                    if (Parameters != null && Parameters.Length > 0)
                    {
                        return string.Format(format, Parameters);
                    }

                    return format;
                }
                catch (Exception)
                {
                    return $"[{Key}]";
                }
            }
        }

        private void OnLanguageChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Trigger property change notification for Value property
            OnPropertyChanged(nameof(Value));
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Return a binding to the Value property
            var binding = new Binding(nameof(Value))
            {
                Source = this,
                Mode = BindingMode.OneWay
            };

            return binding.ProvideValue(serviceProvider);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Cleanup when object is disposed
        ~DynamicLocalizeExtension()
        {
            if (LocalizationService.Instance != null)
            {
                LocalizationService.Instance.PropertyChanged -= OnLanguageChanged;
            }
        }
    }
}
