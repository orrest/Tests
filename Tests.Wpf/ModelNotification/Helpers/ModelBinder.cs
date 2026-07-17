using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.ModelNotification.Helpers;

/// <summary>
/// 通用属性转发辅助类。用于"代理属性"场景：
/// 当 Model 的某个属性变化时，自动触发 ViewModel 对应代理属性（及任意派生属性）的通知，
/// 从而消除每个 ViewModel 里手写 <c>if (e.PropertyName == ...)</c> 的样板代码。
/// </summary>
public static class ModelBinder
{
    /// <summary>
    /// 订阅 <paramref name="model"/> 的属性变化，当 <paramref name="modelProp"/> 指向的属性变化时，
    /// 调用 <paramref name="vm"/> 上的 <c>OnPropertyChanged</c>（通过反射）触发
    /// <paramref name="vmProp"/> 指向的代理属性，以及 <paramref name="extraNotify"/> 中的派生属性。
    /// </summary>
    /// <typeparam name="TModel">Model 类型，必须实现 INotifyPropertyChanged。</typeparam>
    /// <typeparam name="TValue">Model 属性值类型。</typeparam>
    /// <param name="vm">需要触发通知的 ViewModel（通常是 ObservableObject 派生类）。</param>
    /// <param name="model">被监听的 Model 实例。</param>
    /// <param name="modelProp">形如 <c>m =&gt; m.Name</c> 的表达式，定位 Model 属性。</param>
    /// <param name="vmProp">形如 <c>() =&gt; Name</c> 的表达式，定位 ViewModel 代理属性。</param>
    /// <param name="extraNotify">额外的派生属性表达式（如 <c>() =&gt; Greeting</c>），随代理属性一起被通知。</param>
    public static void ForwardProperty<TModel, TValue>(
        ObservableObject vm,
        TModel model,
        Expression<Func<TModel, TValue>> modelProp,
        Expression<Func<TValue>> vmProp,
        params Expression<Func<object>>[] extraNotify)
        where TModel : INotifyPropertyChanged
    {
        if (model is null)
        {
            return;
        }

        string modelPropName = GetMemberName(modelProp);
        string vmPropName = GetMemberName(vmProp);
        string[] extraNames = extraNotify.Select(GetMemberName).ToArray();

        // 拿到 ObservableObject 的 OnPropertyChanged(string) 反射句柄（protected，需反射）。
        MethodInfo? raiseMethod = typeof(ObservableObject).GetMethod(
            "OnPropertyChanged",
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: [typeof(string)],
            modifiers: null);

        model.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != modelPropName)
            {
                return;
            }

            raiseMethod?.Invoke(vm, [vmPropName]);
            foreach (string name in extraNames)
            {
                raiseMethod?.Invoke(vm, [name]);
            }
        };
    }

    /// <summary>
    /// 一次性建立 <paramref name="model"/> 多个属性 → ViewModel 多个代理属性（+各自派生属性）的转发映射。
    /// <para>
    /// 嵌套场景下一个 Model 有多个属性都要转发（如 Address 的 City/Country/ZipCode），
    /// 用此方法比多次调用 <see cref="ForwardProperty{TModel,TValue}"/> 更紧凑：
    /// 只订阅一次 <c>PropertyChanged</c>，按属性名分发到对应的 VM 属性集合。
    /// </para>
    /// <para>
    /// 每个映射项是三元组：(Model 属性选择器, VM 代理属性选择器, 该映射附带的派生属性选择器数组)。
    /// </para>
    /// </summary>
    /// <typeparam name="TModel">Model 类型，必须实现 INotifyPropertyChanged。</typeparam>
    /// <param name="vm">需要触发通知的 ViewModel（ObservableObject 派生类）。</param>
    /// <param name="model">被监听的 Model 实例。</param>
    /// <param name="mappings">映射项数组。每项形如：<c>(m =&gt; m.City, () =&gt; City, [() =&gt; Summary])</c>。</param>
    public static void ForwardProperties<TModel>(
        ObservableObject vm,
        TModel model,
        params (Expression<Func<TModel, object?>> ModelProp,
                Expression<Func<object?>> VmProp,
                Expression<Func<object>>[] ExtraNotify)[] mappings)
        where TModel : INotifyPropertyChanged
    {
        if (model is null || mappings.Length == 0)
        {
            return;
        }

        MethodInfo? raiseMethod = typeof(ObservableObject).GetMethod(
            "OnPropertyChanged",
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: [typeof(string)],
            modifiers: null);

        // 预编译映射表：Model 属性名 → 需通知的 VM 属性名集合（代理 + 派生）
        var dispatch = new Dictionary<string, string[]>(mappings.Length);
        foreach (var (modelProp, vmProp, extraNotify) in mappings)
        {
            string modelPropName = GetMemberName(modelProp);
            string vmPropName = GetMemberName(vmProp);
            var targets = new string[1 + (extraNotify?.Length ?? 0)];
            targets[0] = vmPropName;
            if (extraNotify is { Length: > 0 })
            {
                for (int i = 0; i < extraNotify.Length; i++)
                {
                    targets[i + 1] = GetMemberName(extraNotify[i]);
                }
            }
            dispatch[modelPropName] = targets;
        }

        model.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is null || !dispatch.TryGetValue(e.PropertyName, out var targets))
            {
                return;
            }

            foreach (string name in targets)
            {
                raiseMethod?.Invoke(vm, [name]);
            }
        };
    }

    private static string GetMemberName(LambdaExpression expression)
    {
        // 处理 () => Name 和 () => (object)Name（即 Expression<Func<object>>）两种形式
        Expression body = expression.Body;
        if (body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
        {
            body = unary.Operand;
        }

        return body is MemberExpression member
            ? member.Member.Name
            : throw new ArgumentException("Expression must point to a property or field.", nameof(expression));
    }
}
