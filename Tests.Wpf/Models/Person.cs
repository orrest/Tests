using System.ComponentModel;

namespace Tests.Wpf.Models;

public class Person : IDataErrorInfo
{
    public int Age { get; set; }
    public string Error => string.Empty;

    public string this[string name]
    {
        get
        {
            var result = string.Empty;

            if (name == "Age")
            {
                if (Age < 0 || Age > 150)
                {
                    result = "Age must not be less than 0 or greater than 150.";
                }
            }

            return result;
        }
    }
}
