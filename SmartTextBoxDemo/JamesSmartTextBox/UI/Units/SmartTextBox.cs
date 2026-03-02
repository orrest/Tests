using System.Windows;
using System.Windows.Controls;
using JamesSmartTextBox.Local.Validators;

namespace JamesSmartTextBox.UI.Units;

public class SmartTextBox : TextBox
{
    static SmartTextBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(SmartTextBox),
            new FrameworkPropertyMetadata(typeof(SmartTextBox)));
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(SmartTextBox),
            new PropertyMetadata(string.Empty));

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.Register(
            nameof(Placeholder),
            typeof(string),
            typeof(SmartTextBox),
            new PropertyMetadata(string.Empty));

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(SmartTextBox),
            new PropertyMetadata(new CornerRadius(0)));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty ValidatorProperty =
        DependencyProperty.Register(
            nameof(Validator),
            typeof(IValidator),
            typeof(SmartTextBox),
            new PropertyMetadata(null));

    public IValidator? Validator
    {
        get => (IValidator?)GetValue(ValidatorProperty);
        set => SetValue(ValidatorProperty, value);
    }

    public static readonly DependencyProperty IsValidProperty =
        DependencyProperty.Register(
            nameof(IsValid),
            typeof(bool),
            typeof(SmartTextBox),
            new PropertyMetadata(true));

    public bool IsValid
    {
        get => (bool)GetValue(IsValidProperty);
        set => SetValue(IsValidProperty, value);
    }

    public static readonly DependencyProperty ErrorMessageProperty =
        DependencyProperty.Register(
            nameof(ErrorMessage),
            typeof(string),
            typeof(SmartTextBox),
            new PropertyMetadata(string.Empty));

    public string ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    protected override void OnTextChanged(TextChangedEventArgs e)
    {
        base.OnTextChanged(e);

        if (Validator is not null)
        {
            IsValid = Validator.Validate(Text);
            ErrorMessage = IsValid ? string.Empty : Validator.Message;
        }
    }
}
