﻿using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DentalCore.Wpf.Controls.Converters;

public class SolidBrushToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            return brush.Color;
        }

        return new Color();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}