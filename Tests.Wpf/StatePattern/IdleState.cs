namespace Tests.Wpf.StatePattern;

// Idle state: nothing selected, the panel is empty; Add enters AddingState, selecting a row enters UpdatingState.
// 空闲状态：未选中任何行，面板为空；Add 进入新增状态，选中行进入编辑状态。
public sealed class IdleState : IEditorState
{
    public string DisplayName
    {
        get { return "Idle"; }
    }

    public void OnEnter(StateEditorContext context)
    {
        // Deselect so the grid and the context never disagree about being idle.
        // 清除选中，保证 DataGrid 与 Context 对“空闲”的认知一致。
        context.SelectedRow = null;
        context.ClearEditor();
        context.AddEnabled = true;
        context.UpdateEnabled = false;
        context.DeleteEnabled = false;
        context.AddButtonLabel = "Add";
    }

    public void AddClicked(StateEditorContext context)
    {
        context.ChangeState(new AddingState());
    }

    // Disabled in this state; kept empty to satisfy the uniform interface.
    // 本状态下按钮已禁用；留空以保持接口统一。
    public void UpdateClicked(StateEditorContext context)
    {
    }

    public void DeleteClicked(StateEditorContext context)
    {
    }

    public void SelectionChanged(StateEditorContext context)
    {
        if (context.SelectedRow is not null)
        {
            context.ChangeState(new UpdatingState());
        }
    }
}
