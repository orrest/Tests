using System.ComponentModel.DataAnnotations;

namespace Tests.Wpf.Validations;

class GreatherThanAttribute(string propertyName) : ValidationAttribute
{
    public string PropertyName { get; } = propertyName;

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return new("The current value is null");
        }

        object instance = validationContext.ObjectInstance;
        var otherValue = instance.GetType()?.GetProperty(PropertyName)?.GetValue(instance);

        if (otherValue is null)
        {
            return new("The value to be compared is null");
        }

        if (((IComparable)value).CompareTo(otherValue) > 0)
        {
            return ValidationResult.Success!;
        }

        return new($"The current value {value} is smaller than the {otherValue}");
    }
}
