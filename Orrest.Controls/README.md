# Orrest.Controls

WPF 组件库。引用此程序集的 WPF 应用将自动加载其默认样式。

## 工作原理

1. **Themes/Generic.xaml** — WPF 约定主题资源字典，存放组件库的默认样式。
2. **AssemblyInfo.cs** — `[ThemeInfo]` 特性告诉 WPF 运行时在源程序集的 `Themes/Generic.xaml` 中查找主题资源。

## 用法

在目标 WPF 应用的 `.csproj` 中添加项目引用：

```xml
<ItemGroup>
  <ProjectReference Include="..\Orrest.Controls\Orrest.Controls.csproj" />
</ItemGroup>
```

引用后，该应用会自动加载 `Themes/Generic.xaml` 中定义的所有样式和资源，无需额外 XAML 合并。

## 添加控件样式

在 `Themes/Generic.xaml` 中添加自定义控件的默认样式，例如：

```xml
<Style TargetType="{x:Type local:MyButton}">
  <Setter Property="Background" Value="DodgerBlue" />
</Style>
```
