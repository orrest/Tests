using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Tests.Wpf.Validations;

namespace Tests.Wpf.ValidationRules;

// Row-level rule for DataGrid.RowValidationRules: with ValidationStep=UpdatedValue,
// the value passed to Validate is the row's BindingGroup, whose Items[0] is the
// whole row object — so cross-property rules run when the user commits the row.
// 用于 DataGrid.RowValidationRules 的行级规则：ValidationStep=UpdatedValue 时
// 传入 Validate 的 value 是该行的 BindingGroup，Items[0] 即整行数据对象，
// 因此可在用户提交行时执行跨属性验证。
public class GridRowValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is not BindingGroup bindingGroup || bindingGroup.Items.Count == 0 || bindingGroup.Items[0] is not GridRowItem item)
        {
            return ValidationResult.ValidResult;
        }

        if (item.DiscountPrice >= item.Price)
        {
            return new ValidationResult(false, "Discount price must be lower than the price.");
        }

        return ValidationResult.ValidResult;
    }
}
