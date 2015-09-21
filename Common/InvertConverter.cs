using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Mathe1.Common
{
    public class InvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool) && !(value is Visibility))
                throw new ArgumentException("Kein Boolean, Visibility Typ übergeben");

            if (value is bool)
                return !(bool)value;

            if (value is Visibility)
                return (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
