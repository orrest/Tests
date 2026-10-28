namespace Tests.Wpf.StatePattern;

// Updating state: the editor mirrors the selected row; Update commits the draft, Delete removes the row,
// and Add clears the panel to start a fresh draft in AddingState.
// 编辑状态：编辑区镜像选中行；Update 提交草稿，Delete 删除该行，Add 清空面板重新进入新增状态。
public sealed class UpdatingState : IEditorState
{
    public string DisplayName
    {
        get { return "Updating"; }
    }

    public void OnEnter(StateEditorContext context)
    {
        var row = context.SelectedRow;
        if (row is null)
        {
            // Defensive: entering this state without a selection falls back to idle.
            // 防御：无选中时进入本状态则回退到空闲。
            context.ChangeState(new IdleState());
            return;
        }

        context.LoadEditorFrom(row);
        context.AddEnabled = true;
        context.UpdateEnabled = true;
        context.DeleteEnabled = true;
        context.AddButtonLabel = "Add";
    }

    public void AddClicked(StateEditorContext context)
    {
        context.ChangeState(new AddingState());
    }

    public void UpdateClicked(StateEditorContext context)
    {
        var row = context.SelectedRow;
        if (row is null)
        {
            context.ChangeState(new IdleState());
            return;
        }

        var draft = context.TryCreateRowFromEditor(out var error);
        if (draft is null)
        {
            context.StatusText = error;
            return;
        }

        // Commit the draft; INPC on the row refreshes the DataGrid by itself.
        // 提交草稿；行上的 INPC 让 DataGrid 自动刷新。
        row.Name = draft.Name;
        row.Category = draft.Category;
        row.Price = draft.Price;
        context.StatusText = $"Updated '{row.Name}'.";
    }

    public void DeleteClicked(StateEditorContext context)
    {
        var row = context.SelectedRow;
        if (row is null)
        {
            context.ChangeState(new IdleState());
            return;
        }

        context.Rows.Remove(row);
        // Clearing the selection routes through SelectionChanged and lands in IdleState.
        // 清除选中走 SelectionChanged 流程，自然回到空闲状态。
        context.SelectedRow = null;
        context.StatusText = $"Deleted '{row.Name}'.";
    }

    public void SelectionChanged(StateEditorContext context)
    {
        if (context.SelectedRow is null)
        {
            context.ChangeState(new IdleState());
            return;
        }

        // Another row was clicked: re-mirror the editor and stay in this state.
        // 点了另一行：编辑区重新镜像该行，状态保持不变。
        context.LoadEditorFrom(context.SelectedRow);
    }
}
