using System.Windows;

namespace Tests.Wpf.Controls;

public class CellTexts : System.Windows.Controls.Control
{
    public string Unit
    {
        get { return (string)GetValue(UnitProperty); }
        set { SetValue(UnitProperty, value); }
    }

    public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(
        "Unit",
        typeof(string),
        typeof(CellTexts),
        new PropertyMetadata(string.Empty)
    );

    public double Value
    {
        get { return (double)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        "Value",
        typeof(double),
        typeof(CellTexts),
        new PropertyMetadata(0d)
    );

    public double Gap
    {
        get { return (double)GetValue(GapProperty); }
        set { SetValue(GapProperty, value); }
    }

    public static readonly DependencyProperty GapProperty = DependencyProperty.Register(
        "Gap",
        typeof(double),
        typeof(CellTexts),
        new PropertyMetadata(5d)
    );
}
