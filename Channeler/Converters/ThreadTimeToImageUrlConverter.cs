using Channeler.ViewModel;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Channeler.Converters
{
    public class ThreadTimeToImageUrlConverter : IValueConverter
    {
        private const string BASE_URL = "https://i.4cdn.org/";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const string board = "a/";
            string imageUnixTime = value.ToString();
            string retString = BASE_URL + board + imageUnixTime + "s.jpg";
            return retString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
