using System;
using System.Globalization;
using Xamarin.Forms;
namespace SuperEconomicoApp.Converters
{
    internal class TextToBoolCOnverter : IValueConverter
    {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        if (value == null)
            return false;
        else
            return true;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
}
