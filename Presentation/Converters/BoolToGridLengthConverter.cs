using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Documentor.Presentation.Converters;

public class BoolToGridLengthConverter : IValueConverter
{
    public GridLength TrueValue { get; set; } = new GridLength(280);
    public GridLength FalseValue { get; set; } = new GridLength(80);

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return boolValue ? TrueValue : FalseValue;

        return TrueValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}