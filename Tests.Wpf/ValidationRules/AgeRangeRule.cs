using System.Globalization;
using System.Windows.Controls;

namespace Tests.Wpf.ValidationRules;

public class AgeRangeRule : ValidationRule
{
    public int Min { get; set; }
    public int Max { get; set; }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var age = 0;

        try
        {
            if (((string)value).Length > 0)
                age = int.Parse((string)value);
        }
        catch (Exception e)
        {
            return new ValidationResult(false, "Illegal characters or " + e.Message);
        }

        if ((age < Min) || (age > Max))
        {
            return new ValidationResult(false,
                "Please enter an age in the range: " + Min + " - " + Max + ".");
        }
        return new ValidationResult(true, null);
    }
}
