using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleWpfToolkit;

public class Modal : ContentControl
{
    public static readonly DependencyProperty IsOpenProperty;
    public static readonly DependencyProperty CornerRadiusProperty;
    public static readonly DependencyProperty OuterBackgroundProperty;

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }
    
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
    
    public Brush OuterBackground
    {
        get => (Brush)GetValue(OuterBackgroundProperty);
        set => SetValue(OuterBackgroundProperty, value);
    }

    static Modal()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Modal),
            new FrameworkPropertyMetadata(typeof(Modal)));

        IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen),
            typeof(bool),
            typeof(Modal),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None));
        
        CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius), 
            typeof(CornerRadius), 
            typeof(Modal), 
            new FrameworkPropertyMetadata());
        
        OuterBackgroundProperty = DependencyProperty.Register(
            nameof(OuterBackground), 
            typeof(Brush), 
            typeof(Modal), 
            new FrameworkPropertyMetadata());
    }
}