using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Tests.Wpf.Controls;

public class EditableText : System.Windows.Controls.Control
{
    private TextBox? _textBox;

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(EditableText),
        new FrameworkPropertyMetadata(
            defaultValue: string.Empty,
            flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            propertyChangedCallback: null,
            coerceValueCallback: null
        ));

    /// <summary>
    /// This is a two-way binding for the contorl to use TextBlock to show,
    /// or use TextBox to edit.
    /// </summary>
    public bool IsEditing
    {
        get { return (bool)GetValue(IsEditingProperty); }
        set { SetValue(IsEditingProperty, value); }
    }

    public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register(
        nameof(IsEditing),
        typeof(bool),
        typeof(EditableText),
        new FrameworkPropertyMetadata(
            defaultValue: false,
            flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            propertyChangedCallback: null,
            coerceValueCallback: null
        ));

    public ICommand Command
    {
        get { return (ICommand)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    // Using a DependencyProperty as the backing store for RenameCommand.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command),
        typeof(ICommand),
        typeof(EditableText),
        new PropertyMetadata(null)
    );

    public IRelayCommand RenameFinishedCommand
    {
        get { return (IRelayCommand)GetValue(RenameFinishedCommandProperty); }
    }

    // Using a DependencyProperty as the backing store for RenameFinishedCommand.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty RenameFinishedCommandProperty =
        DependencyProperty.Register(
            nameof(RenameFinishedCommand),
            typeof(IRelayCommand),
            typeof(EditableText),
            new PropertyMetadata(null)
        );

    public EditableText()
    {
        SetValue(RenameFinishedCommandProperty, new RelayCommand(RenameFinished));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_textBox is not null)
            _textBox.LostKeyboardFocus -= OnTextBoxLostKeyboardFocus;

        _textBox = GetTemplateChild("PART_TextBox") as TextBox;

        if (_textBox is not null)
            _textBox.LostKeyboardFocus += OnTextBoxLostKeyboardFocus;
    }

    private void OnTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        RenameFinished();
    }

    private void RenameFinished()
    {
        // two-way binding, so there is no matter if the 
        // outside view mode not set this to false after
        // rename operation
        IsEditing = false;

        bool? canExecute = Command?.CanExecute(this);
        if (canExecute == true)
        {
            Command?.Execute(this);
        }
    }
}
