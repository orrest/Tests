# Tests.Wpf.AutomationPeer

复现 MSDN 存档帖 [WPF memory leak with Automation Peer](https://learn.microsoft.com/en-us/archive/msdn-technet-forums/a480f135-4179-422f-96b9-60a9f5a2722b) 中报告的现象：
`TabControl` 添加 `TabItem` → 全部移除 → 强制 GC，`TabItem` 仍然存活，被 UI Automation 基础设施（AutomationPeer 相关对象）持有。

配合 JetBrains dotMemory 抓快照，观察 GC 根引用链。

## 操作步骤（dotMemory 工作流）

1. 用 dotMemory 启动本应用（或附加到已运行进程）。
2. 抓第 1 个快照（baseline）。
3. 点击 **Add** 数次（每批 10 个 `TabItem`，Header 为 `wwww-0`、`wwww-1`… 便于按实例筛选）。
4. （对照实验的关键开关）视目的决定是否点击 **Simulate UIA Query**：
   - 不点：模拟"纯原帖流程"，验证默认惰性策略下是否已经创建 Peer；
   - 点：等价于 Inspect.exe 等辅助工具连接过应用（`AutomationElement.FromHandle` + `FindAll`），强制 WPF 实例化整棵 AutomationPeer 树，复现确定性更高。
5. 点击 **Remove**（执行原帖同款序列：`SelectedItem = null; SelectedValue = null; Items.Clear()`）。
6. 点击 **GC**（`GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();`），界面显示「存活 X / 总数 Y」。
7. 抓第 2 个快照，与 baseline 对比：
   - 按类型筛 `TabItem`，或按 Header 字符串筛 `wwww-0` 等实例；
   - 对仍存活的对象打开 **Paths to GC Roots**（retention path），观察引用链。

## 预期观察到的滞留链

```
GC Root
  ↓
UI Automation infrastructure (provider/ElementProxy 等)
  ↓
TabControlAutomationPeer / ItemsControlItemAutomationPeer
  ↓
ItemAutomationPeer (per TabItem)
  ↓
TabItem  ← Header: "wwww-N"
```

## 注意事项

- GC 是非确定性的：即使没有强引用，对象也可能因尚未收集 / Finalization 未完成而暂时存活；连续点几次 GC 后再判读。
- 抓快照期间不要运行 Inspect.exe、Narrator、FlaUI 等辅助工具——它们会自然触发 Peer 创建，污染对照结论（与 Simulate UIA Query 同效）。
- 原帖用的 `GC.WaitForFullGCComplete()` 在工作站 GC 配置下不可靠，本工程改用 Collect-Finalize-Collect 三连。
- 泄漏与否取决于 Peer 是否已被创建：Peers 是懒创建的，纯 Add/Remove 在未选中、无 UIA 客户端时未必触发。先跑"不点 Simulate UIA Query"流程、再跑"点了"的流程，对比两次快照是本次实验的核心。

## 相关问题与参考

- 原帖：[WPF memory leak with Automation Peer（MSDN 存档）](https://learn.microsoft.com/en-us/archive/msdn-technet-forums/a480f135-4179-422f-96b9-60a9f5a2722b)
- [dotnet/wpf #10784 — The ItemsControlItemAutomationPeer leads to memory leaks](https://github.com/dotnet/wpf/issues/10784)（.NET 8+ 仍开放）
- [dotnet/wpf #11337 — WPF leaks ElementProxy instances when UI Automation is used](https://github.com/dotnet/wpf/issues/11337)（.NET 4.8 ~ .NET 10，修复 PR 已提交）
- [SyncFusion KB — How to release memory held by AutomationPeer](https://support.syncfusion.com/kb/article/3784)（规避法：自定义 Peer 让 `GetChildrenCore()` 返回 null，代价是禁用 UI Automation）
- [Microsoft Learn — AutomationPeer 概览](https://learn.microsoft.com/en-us/dotnet/api/system.windows.automation.peers.automationpeer)
- [Microsoft Learn — UIElementAutomationPeer.CreatePeerForElement](https://learn.microsoft.com/en-us/dotnet/api/system.windows.automation.peers.uielementautomationpeer.createpeerforelement)
