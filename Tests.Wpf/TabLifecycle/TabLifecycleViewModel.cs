using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.TabLifecycle;

// Page viewmodel: injects the viewmodel-first child vms (transient) of section ② and ③
// and exposes them for the TabItem Content bindings in TabLifecycleView.
// 页面 VM：注入 ②③ 两节的 VM-first 子 VM（transient），供 TabItem Content 绑定。
public partial class TabLifecycleViewModel : ObservableObject
{
    public ViewModelFirst.ContentOneViewModel ContentOne { get; }

    public ViewModelFirst.ContentTwoViewModel ContentTwo { get; }

    public ViewModelFirst.ContentThreeViewModel ContentThree { get; }

    public DirectBinding.DirectOneViewModel DirectOne { get; }

    public DirectBinding.DirectTwoViewModel DirectTwo { get; }

    public DirectBinding.DirectThreeViewModel DirectThree { get; }

    public TabLifecycleViewModel(
        ViewModelFirst.ContentOneViewModel contentOne,
        ViewModelFirst.ContentTwoViewModel contentTwo,
        ViewModelFirst.ContentThreeViewModel contentThree,
        DirectBinding.DirectOneViewModel directOne,
        DirectBinding.DirectTwoViewModel directTwo,
        DirectBinding.DirectThreeViewModel directThree)
    {
        ContentOne = contentOne;
        ContentTwo = contentTwo;
        ContentThree = contentThree;
        DirectOne = directOne;
        DirectTwo = directTwo;
        DirectThree = directThree;
    }
}
