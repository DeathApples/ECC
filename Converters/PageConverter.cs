using System.Globalization;
using System.Windows.Data;

using ECC.Models;

namespace ECC.Converters
{
    internal class PageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PageEnum currentPage && Enum.TryParse(parameter?.ToString(), out PageEnum targetPage))
                return currentPage == targetPage;

            throw new InvalidCastException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool isChecked || !Enum.TryParse(parameter?.ToString(), out PageEnum page))
                throw new InvalidCastException();

            if (!isChecked) return Binding.DoNothing;

            ChangePageEvent?.Invoke(page);
            return page;
        }

        public static event Action<PageEnum>? ChangePageEvent;
    }
}
