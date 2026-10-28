namespace Tests.Wpf.StatePattern;

// Navigation shell; all behavior lives in the StateEditorContext (the State pattern Context role).
// 导航外壳；全部行为都在 StateEditorContext（State 模式的 Context 角色）中。
public sealed class StatePatternViewModel
{
    public StateEditorContext Context { get; } = new();
}
