using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DentalCore.Wpf.Services.Navigation;

namespace DentalCore.Wpf.Converters;

public class InverseViewTypeEqualsToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (ViewType)value == (ViewType)parameter ? Visibility.Hidden : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}