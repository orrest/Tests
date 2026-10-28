namespace Tests.Wpf.StatePattern;

// State interface (State pattern): declares how the panel reacts to every kind of user input.
// 状态接口（State 模式）：声明面板对每一种用户输入的反应。
public interface IEditorState
{
    // Short name rendered in the state indicator.
    // 显示在状态指示器上的短名称。
    string DisplayName { get; }

    // Called by the context right after switching into this state, to sync buttons and editor fields.
    // 由 Context 在切入该状态后立即调用，用于同步按钮可用性与编辑区字段。
    void OnEnter(StateEditorContext context);

    // Semantics of the three buttons in this state.
    // 本状态下三个按钮各自的语义。
    void AddClicked(StateEditorContext context);

    void UpdateClicked(StateEditorContext context);

    void DeleteClicked(StateEditorContext context);

    // Semantics of the DataGrid selection change in this state.
    // 本状态下 DataGrid 选中变化对应的语义。
    void SelectionChanged(StateEditorContext context);
}
