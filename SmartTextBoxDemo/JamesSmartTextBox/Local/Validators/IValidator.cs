using System.Text.RegularExpressions;

namespace JamesSmartTextBox.Local.Validators;

public interface IValidator
{
    string Message { get; }
    bool Validate(string value);
}

public class RequiredValidator : IValidator
{
    public string Message { get; set; } = "This field is required.";

    public bool Validate(string value) => !string.IsNullOrWhiteSpace(value);
}

public class MinLengthValidator : IValidator
{
    public int MinLength { get; set; } = 1;

    public string Message { get; set; } = "Value is too short.";

    public bool Validate(string value) => value?.Length >= MinLength;
}

public partial class EmailValidator : IValidator
{
    public string Message { get; set; } = "Invalid email address.";

    public bool Validate(string value) =>
        !string.IsNullOrWhiteSpace(value) && EmailRegex().IsMatch(value);

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}

public partial class PasswordValidator : IValidator
{
    public string Message { get; set; } = "Password must be at least 8 characters with uppercase, lowercase, and a number.";

    public bool Validate(string value) =>
        !string.IsNullOrWhiteSpace(value) && PasswordRegex().IsMatch(value);

    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")]
    private static partial Regex PasswordRegex();
}

public partial class UrlValidator : IValidator
{
    public string Message { get; set; } = "Invalid URL.";

    public bool Validate(string value) =>
        !string.IsNullOrWhiteSpace(value) && UrlRegex().IsMatch(value);

    [GeneratedRegex(@"^https?://[^\s/$.?#].[^\s]*$", RegexOptions.IgnoreCase)]
    private static partial Regex UrlRegex();
}

public partial class PassportValidator : IValidator
{
    public string Message { get; set; } = "Invalid passport number.";

    public bool Validate(string value) =>
        !string.IsNullOrWhiteSpace(value) && PassportRegex().IsMatch(value);

    [GeneratedRegex(@"^[A-Z]{1,2}\d{6,9}$", RegexOptions.IgnoreCase)]
    private static partial Regex PassportRegex();
}

public partial class CreditCardValidator : IValidator
{
    public string Message { get; set; } = "Invalid credit card number.";

    public bool Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        string digits = DigitsOnly().Replace(value, "");

        if (digits.Length < 13 || digits.Length > 19)
            return false;

        int sum = 0;
        bool alternate = false;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int n = digits[i] - '0';
            if (alternate)
            {
                n *= 2;
                if (n > 9)
                    n -= 9;
            }
            sum += n;
            alternate = !alternate;
        }
        return sum % 10 == 0;
    }

    [GeneratedRegex(@"[^\d]")]
    private static partial Regex DigitsOnly();
}
