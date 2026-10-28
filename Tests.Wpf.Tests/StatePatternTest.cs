using System.Globalization;
using Tests.Wpf.StatePattern;

namespace Tests.Wpf.Tests;

// Covers the State pattern state machine of the editor panel: every user input (button click,
// row selection) is delegated to the current state, which decides behavior and the next transition.
// 覆盖编辑面板的 State 模式状态机：每种用户输入（按钮点击、行选中）都委托给当前状态，
// 由状态决定行为与下一次转移。
[TestClass]
public sealed class StatePatternTest
{
    private static StateEditorContext CreateContext()
    {
        return new StateEditorContext();
    }

    // Enters UpdatingState exactly the way the UI does: assign SelectedRow, the context routes
    // it through the current state's SelectionChanged.
    // 与 UI 完全一致地进入编辑状态：直接赋值 SelectedRow，Context 会路由到当前状态的 SelectionChanged。
    private static ProductRow EnterUpdating(StateEditorContext context)
    {
        var row = context.Rows[0];
        context.SelectedRow = row;
        return row;
    }

    private static void EnterAdding(StateEditorContext context)
    {
        context.AddCommand.Execute(null);
    }

    [TestMethod]
    public void InitialStateIsIdleWithOnlyAddEnabled()
    {
        var context = CreateContext();

        Assert.AreEqual("Idle", context.StateName);
        Assert.IsNull(context.SelectedRow);
        Assert.IsTrue(context.AddEnabled);
        Assert.IsFalse(context.UpdateEnabled);
        Assert.IsFalse(context.DeleteEnabled);
        Assert.AreEqual("Add", context.AddButtonLabel);
    }

    [TestMethod]
    public void IdleAddClickEntersAddingAndClearsPanel()
    {
        var context = CreateContext();

        EnterAdding(context);

        Assert.AreEqual("Adding", context.StateName);
        Assert.AreEqual(string.Empty, context.EditorName);
        Assert.AreEqual(string.Empty, context.EditorPrice);
        Assert.IsNull(context.SelectedRow);
        Assert.AreEqual("Save", context.AddButtonLabel);
        Assert.IsTrue(context.AddEnabled);
        Assert.IsFalse(context.UpdateEnabled);
        Assert.IsFalse(context.DeleteEnabled);
    }

    [TestMethod]
    public void IdleRowSelectionEntersUpdatingAndMirrorsRow()
    {
        var context = CreateContext();
        var row = context.Rows[1];

        context.SelectedRow = row;

        Assert.AreSame(row, context.SelectedRow);
        Assert.AreEqual("Updating", context.StateName);
        Assert.AreEqual(row.Name, context.EditorName);
        Assert.AreEqual(row.Category, context.EditorCategory);
        Assert.AreEqual(row.Price.ToString(CultureInfo.InvariantCulture), context.EditorPrice);
        Assert.IsTrue(context.UpdateEnabled);
        Assert.IsTrue(context.DeleteEnabled);
        Assert.AreEqual("Add", context.AddButtonLabel);
    }

    [TestMethod]
    public void AddingSaveClickAppendsRowAndSelectsIt()
    {
        var context = CreateContext();
        EnterAdding(context);
        context.EditorName = "Keyboard";
        context.EditorCategory = "Hardware";
        context.EditorPrice = "12.5";
        var countBefore = context.Rows.Count;

        context.AddCommand.Execute(null);

        Assert.HasCount(countBefore + 1, context.Rows);
        var added = context.Rows[^1];
        Assert.AreSame(added, context.SelectedRow);
        Assert.AreEqual("Updating", context.StateName);
        Assert.AreEqual("Keyboard", added.Name);
        Assert.AreEqual(12.5m, added.Price);
    }

    [TestMethod]
    public void AddingSaveClickWithMissingNameKeepsAddingAndShowsError()
    {
        var context = CreateContext();
        EnterAdding(context);

        context.AddCommand.Execute(null);

        Assert.AreEqual("Adding", context.StateName);
        Assert.HasCount(5, context.Rows);
        Assert.IsFalse(string.IsNullOrEmpty(context.StatusText));
    }

    [TestMethod]
    public void AddingSaveClickWithInvalidPriceKeepsAdding()
    {
        var context = CreateContext();
        EnterAdding(context);
        context.EditorName = "Keyboard";
        context.EditorPrice = "abc";

        context.AddCommand.Execute(null);

        Assert.AreEqual("Adding", context.StateName);
        Assert.HasCount(5, context.Rows);
    }

    [TestMethod]
    public void AddingRowSelectionWinsOverDraft()
    {
        var context = CreateContext();
        EnterAdding(context);
        context.EditorName = "draft";

        context.SelectedRow = context.Rows[2];

        Assert.AreEqual("Updating", context.StateName);
        Assert.AreEqual(context.Rows[2].Name, context.EditorName);
    }

    [TestMethod]
    public void UpdatingAddClickClearsPanelAndEntersAdding()
    {
        var context = CreateContext();
        EnterUpdating(context);

        EnterAdding(context);

        Assert.AreEqual("Adding", context.StateName);
        Assert.IsNull(context.SelectedRow);
        Assert.AreEqual(string.Empty, context.EditorName);
        Assert.AreEqual("Save", context.AddButtonLabel);
    }

    [TestMethod]
    public void UpdatingUpdateClickCommitsDraftToRow()
    {
        var context = CreateContext();
        var row = EnterUpdating(context);
        context.EditorName = "Marker";
        context.EditorPrice = "2.25";

        context.UpdateCommand.Execute(null);

        Assert.AreSame(row, context.SelectedRow);
        Assert.AreEqual("Updating", context.StateName);
        Assert.AreEqual("Marker", row.Name);
        Assert.AreEqual(2.25m, row.Price);
        Assert.HasCount(5, context.Rows);
    }

    [TestMethod]
    public void UpdatingUpdateClickWithInvalidPriceKeepsRowUnchanged()
    {
        var context = CreateContext();
        var row = EnterUpdating(context);
        var nameBefore = row.Name;
        var priceBefore = row.Price;
        context.EditorPrice = "not-a-number";

        context.UpdateCommand.Execute(null);

        Assert.AreEqual(nameBefore, row.Name);
        Assert.AreEqual(priceBefore, row.Price);
        Assert.AreEqual("Updating", context.StateName);
        Assert.IsFalse(string.IsNullOrEmpty(context.StatusText));
    }

    [TestMethod]
    public void UpdatingDeleteClickRemovesRowAndReturnsToIdle()
    {
        var context = CreateContext();
        var row = EnterUpdating(context);
        var countBefore = context.Rows.Count;

        context.DeleteCommand.Execute(null);

        Assert.HasCount(countBefore - 1, context.Rows);
        CollectionAssert.DoesNotContain(context.Rows, row);
        Assert.IsNull(context.SelectedRow);
        Assert.AreEqual("Idle", context.StateName);
        Assert.AreEqual(string.Empty, context.EditorName);
    }

    [TestMethod]
    public void UpdatingDeselectionReturnsToIdle()
    {
        var context = CreateContext();
        EnterUpdating(context);

        context.SelectedRow = null;

        Assert.AreEqual("Idle", context.StateName);
        Assert.IsFalse(context.UpdateEnabled);
        Assert.IsFalse(context.DeleteEnabled);
        Assert.AreEqual(string.Empty, context.EditorName);
    }

    [TestMethod]
    public void UpdatingSelectingAnotherRowReMirrorsEditor()
    {
        var context = CreateContext();
        EnterUpdating(context);
        var other = context.Rows[3];

        context.SelectedRow = other;

        Assert.AreEqual("Updating", context.StateName);
        Assert.AreSame(other, context.SelectedRow);
        Assert.AreEqual(other.Name, context.EditorName);
    }

    [TestMethod]
    public void TryCreateRowFromEditorTrimsInputsAndParsesInvariantPrice()
    {
        var context = CreateContext();
        context.EditorName = "  Ruler  ";
        context.EditorCategory = " Stationery ";
        context.EditorPrice = " 3.5 ";

        var row = context.TryCreateRowFromEditor(out var error);

        Assert.IsNotNull(row);
        Assert.AreEqual(string.Empty, error);
        Assert.AreEqual("Ruler", row!.Name);
        Assert.AreEqual("Stationery", row.Category);
        Assert.AreEqual(3.5m, row.Price);
    }

    [TestMethod]
    public void TryCreateRowFromEditorRejectsInvalidInputsWithoutThrowing()
    {
        var emptyName = CreateContext();
        emptyName.EditorPrice = "1";
        var missingPrice = CreateContext();
        missingPrice.EditorName = "Pen";
        var badPrice = CreateContext();
        badPrice.EditorName = "Pen";
        badPrice.EditorPrice = "1.2.3";

        Assert.IsNull(emptyName.TryCreateRowFromEditor(out var nameError));
        Assert.IsFalse(string.IsNullOrEmpty(nameError));
        Assert.IsNull(missingPrice.TryCreateRowFromEditor(out var priceError));
        Assert.IsFalse(string.IsNullOrEmpty(priceError));
        Assert.IsNull(badPrice.TryCreateRowFromEditor(out var parseError));
        Assert.IsFalse(string.IsNullOrEmpty(parseError));
    }
}
