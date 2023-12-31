using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleWpfToolkit;

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
            typeof(IntegerTextBox),
            new FrameworkPropertyMetadata(OnValueChanged));

        MinProperty = DependencyProperty.Register(
            nameof(Min),
            typeof(int),
            typeof(IntegerTextBox),
            new FrameworkPropertyMetadata(0));

        MaxProperty = DependencyProperty.Register(
            nameof(Max),
            typeof(int),
            typeof(IntegerTextBox),
            new FrameworkPropertyMetadata(1_000_000_000, OnMaxChanged, CoerceMax));

        ValueChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(ValueChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<int>),
            typeof(IntegerTextBox));
    }

    public IntegerTextBox()
    {
        DataObject.AddPastingHandler(this, OnPasting);
        MaxLength = 10;
    }

    protected override void OnTextChanged(TextChangedEventArgs args)
    {
        Value = int.TryParse(Text, out var num)
            ? CoerceValue(num)
            : default;

        base.OnTextChanged(args);
    }

    protected override void OnPreviewTextInput(TextCompositionEventArgs e)
    {
        var oldText = Text;

        if (oldText.Length >= MaxLength ||
            !int.TryParse(e.Text, out var digit) ||
            IsInvalidZero(digit))
        {
            e.Handled = true;
        }

        return;

        bool IsInvalidZero(int num)
        {
            return num == 0 &&
                   Min > 0 &&
                   string.IsNullOrEmpty(oldText);
        }
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

    private static object CoerceMax(DependencyObject d, object baseValue)
    {
        var control = (IntegerTextBox)d;
        var max = (int)baseValue;

        if (max < 0)
        {
            max = 0;
        }

        if (control.Min > max)
        {
            control.Min = max;
        }

        return max;
    }

    private static void OnMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (IntegerTextBox)d;
        var max = (int)e.NewValue;

        control.MaxLength = (int)Math.Floor(Math.Log10(max) + 1);
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

    private int CoerceValue(int oldValue)
    {
        if (oldValue < Min)
        {
            return Min;
        }

        return oldValue > Max ? Max : oldValue;
    }
}