using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Tests.Wpf.StatePattern;

// Context (State pattern): keeps the current state, delegates every user input to it, and exposes
// the shared data (rows, draft editor fields, button flags) that states operate on.
// Context（State 模式）：持有当前状态，把所有用户输入委托给它，
// 并暴露各状态操作的共享数据（行集合、编辑区草稿字段、按钮可用标志）。
public partial class StateEditorContext : ObservableObject
{
    public ObservableCollection<ProductRow> Rows { get; } =
    [
        new() { Name = "Ball pen", Category = "Stationery", Price = 1.5m },
        new() { Name = "Notebook", Category = "Stationery", Price = 3.9m },
        new() { Name = "Stapler", Category = "Office", Price = 7.2m },
        new() { Name = "USB hub", Category = "Accessory", Price = 15.0m },
        new() { Name = "Monitor", Category = "Hardware", Price = 89.9m },
    ];

    // Current state, starts idle; states hold no fields of their own, so a fresh instance
    // is created on demand at every transition.
    // 当前状态，初始为空闲；状态自身不持有独立变量，每次转移时按需手动 new。
    public IEditorState CurrentState { get; private set; } = new IdleState();

    public string StateName
    {
        get { return CurrentState.DisplayName; }
    }

    [ObservableProperty]
    public partial ProductRow? SelectedRow { get; set; }

    [ObservableProperty]
    public partial string EditorName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EditorCategory { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EditorPrice { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool AddEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool UpdateEnabled { get; set; }

    [ObservableProperty]
    public partial bool DeleteEnabled { get; set; }

    [ObservableProperty]
    public partial string AddButtonLabel { get; set; } = "Add";

    [ObservableProperty]
    public partial string StatusText { get; set; } = "Ready.";

    public IRelayCommand AddCommand { get; }

    public IRelayCommand UpdateCommand { get; }

    public IRelayCommand DeleteCommand { get; }

    public StateEditorContext()
    {
        // Button clicks carry no if/else at all: they are forwarded to the current state as-is.
        // 按钮点击不含任何 if/else：原样转发给当前状态处理。
        AddCommand = new RelayCommand(() => CurrentState.AddClicked(this));
        UpdateCommand = new RelayCommand(() => CurrentState.UpdateClicked(this));
        DeleteCommand = new RelayCommand(() => CurrentState.DeleteClicked(this));
        CurrentState.OnEnter(this);
    }

    // The only door for state transitions; OnEnter lets each state sync the panel its own way.
    // 状态切换的唯一入口；OnEnter 让每个状态按自己的方式同步面板。
    public void ChangeState(IEditorState state)
    {
        CurrentState = state;
        OnPropertyChanged(nameof(StateName));
        state.OnEnter(this);
    }

    public void ClearEditor()
    {
        EditorName = string.Empty;
        EditorCategory = string.Empty;
        EditorPrice = string.Empty;
    }

    public void LoadEditorFrom(ProductRow row)
    {
        EditorName = row.Name;
        EditorCategory = row.Category;
        EditorPrice = row.Price.ToString(CultureInfo.InvariantCulture);
    }

    // Validates the draft and returns a new row, or null with an error message.
    // 校验草稿并返回新行；失败时返回 null 及错误信息。
    public ProductRow? TryCreateRowFromEditor(out string error)
    {
        if (string.IsNullOrWhiteSpace(EditorName))
        {
            error = "Name is required.";
            return null;
        }

        if (string.IsNullOrWhiteSpace(EditorPrice))
        {
            error = "Price is required.";
            return null;
        }

        if (!decimal.TryParse(EditorPrice, NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
        {
            error = $"Price '{EditorPrice}' is not a valid number.";
            return null;
        }

        error = string.Empty;
        return new ProductRow { Name = EditorName.Trim(), Category = EditorCategory.Trim(), Price = price };
    }

    // Every DataGrid selection change is just another input delegated to the current state.
    // DataGrid 的每次选中变化只是又一种输入，同样委托给当前状态。
    partial void OnSelectedRowChanged(ProductRow? value)
    {
        CurrentState.SelectionChanged(this);
    }
}
