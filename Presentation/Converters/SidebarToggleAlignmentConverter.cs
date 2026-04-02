using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Documentor.Presentation.Converters;

public class SidebarToggleAlignmentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isExpanded)
            return isExpanded ? HorizontalAlignment.Left : HorizontalAlignment.Center;

        return HorizontalAlignment.Left;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}