using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Resoucres.Services
{
    public class LocalizationService : INotifyPropertyChanged
    {
        private static LocalizationService? _instance;
        private static readonly object _lock = new object();
        public static LocalizationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new LocalizationService();
                    }
                }
                return _instance;
            }
        }

        private LocalizationService()
        {
            // Khởi tạo với ngôn ngữ mặc định
            _currentCulture = Thread.CurrentThread.CurrentUICulture;
        }

        private CultureInfo _currentCulture;
        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            private set
            {
                if (_currentCulture != value)
                {
                    _currentCulture = value;
                    OnPropertyChanged();
                    // Thông báo rằng tất cả localized strings cần được cập nhật
                    OnPropertyChanged("Item[]"); // Này sẽ trigger binding update
                }
            }
        }

        public void ChangeLanguage(string cultureName)
        {
            try
            {
                if(CurrentCulture.Name.Equals(cultureName, StringComparison.OrdinalIgnoreCase)) return;
                var culture = new CultureInfo(cultureName);
                ChangeLanguage(culture);
            }
            catch (CultureNotFoundException)
            {
                // Fallback to English if culture not found
                ChangeLanguage(new CultureInfo("en-US"));
            }
        }

        public void ChangeLanguage(CultureInfo culture)
        {
            CurrentCulture = culture;

            // Cập nhật culture cho thread hiện tại
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            // Cập nhật culture cho tất cả threads mới
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
