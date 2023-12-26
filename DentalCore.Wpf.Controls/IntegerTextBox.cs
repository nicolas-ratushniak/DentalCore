using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DentalCore.Wpf.Controls;

public class IntegerTextBox : PlaceholderTextBox
{
    public static readonly DependencyProperty ValueProperty;
    public static readonly DependencyProperty MinProperty;
    public static readonly DependencyProperty MaxProperty;
    public static readonly RoutedEvent ValueChangedEvent;

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public int Min
    {
        get => (int)GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }

    public int Max
    {
        get => (int)GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    public event RoutedPropertyChangedEventHandler<int> ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    static IntegerTextBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(IntegerTextBox),
            new FrameworkPropertyMetadata(typeof(IntegerTextBox)));

        ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(int),
            typeof(PlaceholderTextBox),
            new FrameworkPropertyMetadata(OnValueChanged, CoerceValue));

        MinProperty = DependencyProperty.Register(
            nameof(Min),
            typeof(int),
            typeof(PlaceholderTextBox),
            new FrameworkPropertyMetadata(0, OnMinChanged, CoerceMin));

        MaxProperty = DependencyProperty.Register(
            nameof(Max),
            typeof(int),
            typeof(PlaceholderTextBox),
            new FrameworkPropertyMetadata(int.MaxValue, OnMaxChanged));

        ValueChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(ValueChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<int>),
            typeof(IntegerTextBox));
    }

    public IntegerTextBox()
    {
        DataObject.AddPastingHandler(this, OnPasting);
    }

    protected override void OnTextChanged(TextChangedEventArgs args)
    {
        Value = int.TryParse(Text, out var num) ? num : default;

        base.OnTextChanged(args);
    }

    protected override void OnPreviewTextInput(TextCompositionEventArgs e)
    {
        var oldText = Text;

        if (!int.TryParse(e.Text, out var digit) ||
            (Min > 0 && string.IsNullOrEmpty(oldText) && digit == 0) ||
            oldText.Length >= MaxLength)
        {
            e.Handled = true;
        }

        base.OnPreviewTextInput(e);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Space)
        {
            e.Handled = true;
        }
        
        base.OnPreviewKeyDown(e);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        Text = Value.ToString();
        
        base.OnLostFocus(e);
    }

    private void OnPasting(object sender, DataObjectPastingEventArgs e)
    {
        var text = (string?)e.DataObject.GetData(typeof(string));

        if (text is null || !Regex.IsMatch(text, @"^\d*$") || text.Length > MaxLength)
        {
            e.CancelCommand();
        }
    }

    private static void OnMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (IntegerTextBox)d;
        var newValue = (int)e.NewValue;

        if (newValue < 0)
        {
            control.Min = 0;
        }

        if (control.Value < control.Min)
        {
            control.Value = control.Min;
        }
    }

    private static object CoerceMin(DependencyObject d, object baseValue)
    {
        var control = (IntegerTextBox)d;
        var baseMin = (int)baseValue;

        return baseMin > control.Max ? control.Max : baseMin;
    }

    private static void OnMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (IntegerTextBox)d;
        var newValue = (int)e.NewValue;

        if (control.Value > newValue)
        {
            control.Value = control.Max;
        }
        
        control.MaxLength = (int)Math.Floor(Math.Log10(control.Max) + 1);
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (IntegerTextBox)d;
        var newValue = (int)e.NewValue;

        if (!control.IsFocused)
        {
            control.Text = newValue.ToString();
        }

        var args = new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, newValue, ValueChangedEvent);
        control.RaiseEvent(args);
    }

    private static object CoerceValue(DependencyObject d, object baseValue)
    {
        var control = (IntegerTextBox)d;
        var value = (int)baseValue;

        if (value < control.Min)
        {
            return control.Min;
        }

        if (value > control.Max)
        {
            return control.Max;
        }

        return value;
    }
}