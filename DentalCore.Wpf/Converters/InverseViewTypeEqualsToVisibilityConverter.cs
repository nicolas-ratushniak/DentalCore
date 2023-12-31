using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DentalCore.Wpf.Abstract;

namespace DentalCore.Wpf.Converters;

public class InverseViewTypeEqualsToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (PageType)value == (PageType)parameter ? Visibility.Hidden : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}