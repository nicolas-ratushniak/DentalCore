using System.Windows;
using System.Windows.Controls;

namespace DentalCore.Wpf.Controls;

public class PlaceholderTextBox : TextBox
{
    public static readonly DependencyProperty PlaceholderProperty;
    public static readonly DependencyProperty CornerRadiusProperty;
    public static readonly DependencyProperty IsEmptyProperty;

    private static readonly DependencyPropertyKey IsEmptyPropertyKey;

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public bool IsEmpty
    {
        get => (bool)GetValue(IsEmptyProperty);
        private set => SetValue(IsEmptyPropertyKey, value);
    }

    static PlaceholderTextBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PlaceholderTextBox),
            new FrameworkPropertyMetadata(typeof(PlaceholderTextBox)));

        PlaceholderProperty = DependencyProperty.Register(
            nameof(Placeholder), 
            typeof(string), 
            typeof(PlaceholderTextBox), 
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None));
        
        CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius), 
            typeof(CornerRadius), 
            typeof(PlaceholderTextBox), 
            new FrameworkPropertyMetadata());

        IsEmptyPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsEmpty),
            typeof(bool),
            typeof(PlaceholderTextBox),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None));

        IsEmptyProperty = IsEmptyPropertyKey.DependencyProperty;
    }

    protected override void OnTextChanged(TextChangedEventArgs args)
    {
        IsEmpty = string.IsNullOrEmpty(Text);
        base.OnTextChanged(args);
    }
}