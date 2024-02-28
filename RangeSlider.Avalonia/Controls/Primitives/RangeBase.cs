using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Utilities;

namespace RangeSlider.Avalonia.Controls.Primitives;

/// <summary>
/// Base class for controls that display a value within a range.
/// </summary>
public abstract class RangeBase : TemplatedControl
{
    private bool _isDataContextChanging;

    /// <summary>
    /// Defines the <see cref="Minimum"/> property.
    /// </summary>
    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<RangeBase, double>(nameof(Minimum), coerce: CoerceMinimum);

    /// <summary>
    /// Defines the <see cref="Maximum"/> property.
    /// </summary>
    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<RangeBase, double>(nameof(Maximum), 100, coerce: CoerceMaximum);

    /// <summary>
    /// Defines the <see cref="LowerValue"/> property.
    /// </summary>
    public static readonly StyledProperty<double> LowerValueProperty =
        AvaloniaProperty.Register<RangeBase, double>(nameof(LowerValue),
            defaultBindingMode: BindingMode.TwoWay,
            coerce: CoerceLowerValue);

    /// <summary>
    /// Defines the <see cref="UpperValue"/> property.
    /// </summary>
    public static readonly StyledProperty<double> UpperValueProperty =
        AvaloniaProperty.Register<RangeBase, double>(nameof(UpperValue),
            defaultBindingMode: BindingMode.TwoWay,
            coerce: CoerceUpperValue);

    /// <summary>
    /// Defines the <see cref="SmallChange"/> property.
    /// </summary>
    public static readonly StyledProperty<double> SmallChangeProperty =
        AvaloniaProperty.Register<RangeBase, double>(nameof(SmallChange), 1);

    /// <summary>
    /// Defines the <see cref="LargeChange"/> property.
    /// </summary>
    public static readonly StyledProperty<double> LargeChangeProperty =
        AvaloniaProperty.Register<RangeBase, double>(nameof(LargeChange), 10);

    /// <summary>
    /// Defines the <see cref="LowerValueChanged"/> event.
    /// </summary>
    public static readonly RoutedEvent<RangeBaseValueChangedEventArgs> LowerValueChangedEvent =
        RoutedEvent.Register<RangeBase, RangeBaseValueChangedEventArgs>(
            nameof(LowerValueChanged), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when the <see cref="LowerValue"/> property changes.
    /// </summary>
    public event EventHandler<RangeBaseValueChangedEventArgs>? LowerValueChanged
    {
        add => AddHandler(LowerValueChangedEvent, value);
        remove => RemoveHandler(LowerValueChangedEvent, value);
    }

    /// <summary>
    /// Defines the <see cref="UpperValueChanged"/> event.
    /// </summary>
    public static readonly RoutedEvent<RangeBaseValueChangedEventArgs> UpperValueChangedEvent =
        RoutedEvent.Register<RangeBase, RangeBaseValueChangedEventArgs>(
            nameof(UpperValueChanged), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when the <see cref="UpperValue"/> property changes.
    /// </summary>
    public event EventHandler<RangeBaseValueChangedEventArgs>? UpperValueChanged
    {
        add => AddHandler(UpperValueChangedEvent, value);
        remove => RemoveHandler(UpperValueChangedEvent, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeBase"/> class.
    /// </summary>
    public RangeBase()
    {
    }

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    public double Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    private static double CoerceMinimum(AvaloniaObject sender, double value)
    {
        return ValidateDouble(value) ? value : sender.GetValue(MinimumProperty);
    }

    private void OnMinimumChanged()
    {
        if (IsInitialized && !_isDataContextChanging)
        {
            CoerceValue(MaximumProperty);
            CoerceValue(LowerValueProperty);
            CoerceValue(UpperValueProperty);
        }
    }


    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    private static double CoerceMaximum(AvaloniaObject sender, double value)
    {
        return ValidateDouble(value)
            ? Math.Max(value, sender.GetValue(MinimumProperty))
            : sender.GetValue(MaximumProperty);
    }

    private void OnMaximumChanged()
    {
        if (IsInitialized && !_isDataContextChanging)
        {
            CoerceValue(MinimumProperty);
            CoerceValue(LowerValueProperty);
            CoerceValue(UpperValueProperty);
        }
    }

    /// <summary>
    /// Gets or sets the lower selected value.
    /// </summary>
    public double LowerValue
    {
        get => GetValue(LowerValueProperty);
        set => SetValue(LowerValueProperty, value);
    }

    private static double CoerceLowerValue(AvaloniaObject sender, double value)
    {
        return ValidateDouble(value)
            ? MathUtilities.Clamp(value, sender.GetValue(MinimumProperty), sender.GetValue(MaximumProperty))
            : sender.GetValue(LowerValueProperty);
    }


    /// <summary>
    /// Gets or sets the upper selected value.
    /// </summary>
    public double UpperValue
    {
        get => GetValue(UpperValueProperty);
        set => SetValue(UpperValueProperty, value);
    }

    private static double CoerceUpperValue(AvaloniaObject sender, double value)
    {
        return ValidateDouble(value)
            ? MathUtilities.Clamp(value, sender.GetValue(MinimumProperty), sender.GetValue(MaximumProperty))
            : sender.GetValue(UpperValueProperty);
    }

    public double SmallChange
    {
        get => GetValue(SmallChangeProperty);
        set => SetValue(SmallChangeProperty, value);
    }

    public double LargeChange
    {
        get => GetValue(LargeChangeProperty);
        set => SetValue(LargeChangeProperty, value);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        CoerceValue(MaximumProperty);
        CoerceValue(LowerValueProperty);
        CoerceValue(UpperValueProperty);
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == MinimumProperty)
        {
            OnMinimumChanged();
        }
        else if (change.Property == MaximumProperty)
        {
            OnMaximumChanged();
        }
        else if (change.Property == LowerValueProperty)
        {
            var valueChangedEventArgs = new RangeBaseValueChangedEventArgs(
                change.GetOldValue<double>(),
                change.GetNewValue<double>(),
                LowerValueChangedEvent);
            RaiseEvent(valueChangedEventArgs);
        }
        else if (change.Property == UpperValueProperty)
        {
            var valueChangedEventArgs = new RangeBaseValueChangedEventArgs(
                change.GetOldValue<double>(),
                change.GetNewValue<double>(),
                UpperValueChangedEvent);
            RaiseEvent(valueChangedEventArgs);
        }
    }

    /// <inheritdoc />
    protected override void OnDataContextBeginUpdate()
    {
        _isDataContextChanging = true;
        base.OnDataContextBeginUpdate();
    }

    /// <inheritdoc />
    protected override void OnDataContextEndUpdate()
    {
        base.OnDataContextEndUpdate();
        _isDataContextChanging = false;
    }

    /// <summary>
    /// Checks if the double value is not infinity nor NaN.
    /// </summary>
    /// <param name="value">The value.</param>
    private static bool ValidateDouble(double value)
    {
        return !double.IsInfinity(value) || !double.IsNaN(value);
    }
}