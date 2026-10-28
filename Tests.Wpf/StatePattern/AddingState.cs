namespace Tests.Wpf.StatePattern;

// Adding state: the panel holds a draft of a brand-new row and the Add button turns into "Save".
// 新增状态：面板保存的是新行草稿，Add 按钮变为 "Save"。
public sealed class AddingState : IEditorState
{
    public string DisplayName
    {
        get { return "Adding"; }
    }

    public void OnEnter(StateEditorContext context)
    {
        // A new draft starts from a clean panel; the grid selection is cleared with it.
        // 新草稿从空面板开始；DataGrid 选中一并清除。
        context.SelectedRow = null;
        context.ClearEditor();
        context.AddEnabled = true;
        context.UpdateEnabled = false;
        context.DeleteEnabled = false;
        context.AddButtonLabel = "Save";
    }

    public void AddClicked(StateEditorContext context)
    {
        // In this state the Add button means "save the draft as a new row".
        // 本状态下 Add 按钮的含义是“把草稿保存为新行”。
        var row = context.TryCreateRowFromEditor(out var error);
        if (row is null)
        {
            context.StatusText = error;
            return;
        }

        context.Rows.Add(row);
        // Selecting the new row routes through SelectionChanged and lands in UpdatingState.
        // 选中新行走 SelectionChanged 流程，自然进入编辑状态。
        context.SelectedRow = row;
        context.StatusText = $"Added '{row.Name}'.";
    }

    public void UpdateClicked(StateEditorContext context)
    {
    }

    public void DeleteClicked(StateEditorContext context)
    {
    }

    public void SelectionChanged(StateEditorContext context)
    {
        // Selecting a row always wins over the unfinished draft.
        // 选中行总是优先于未完成的草稿。
        if (context.SelectedRow is not null)
        {
            context.ChangeState(new UpdatingState());
        }
    }
}
