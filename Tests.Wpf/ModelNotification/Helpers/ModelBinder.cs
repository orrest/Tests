using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.ModelNotification.Helpers;

/// <summary>
/// 通用属性转发辅助类。用于"无代理属性"场景：
/// 当 Model 的某个属性变化时，自动触发 ViewModel 派生属性（如 Summary）的通知，
/// 从而消除每个 ViewModel 里手写 <c>if (e.PropertyName == ...)</c> 的样板代码。
/// <para>
/// View 直接绑定 Model 深链路（<c>{Binding Model.Name}</c>），Model 自身实现 INotifyPropertyChanged，
/// 故代理属性的刷新由 Model 直接通知 View；本类仅负责让 VM 的<b>派生计算属性</b>响应 Model 变化。
/// </para>
/// </summary>
public static class ModelBinder
{
    /// <summary>
    /// 建立 Model 属性变化 → ViewModel 派生属性通知 的映射，返回 <see cref="ModelSubscription"/> 句柄。
    /// <para>
    /// 典型用法（嵌套场景下为每层 Model 各调用一次）：
    /// <code>
    /// _sub?.Dispose();
    /// _sub = ModelBinder.ForwardDerived(
    ///     this, stepModel,
    ///     (m =&gt; m.Name, [() =&gt; Summary]),
    ///     (m =&gt; m.Enabled, [() =&gt; Summary]));
    /// </code>
    /// </para>
    /// <para>
    /// 替换 Model 实例时，Dispose 旧句柄（解绑事件）再调用本方法建立新订阅。
    /// 也可配合 <c>[ObservableProperty]</c> 的 <c>OnXxxChanged</c> partial 方法自动重订阅。
    /// </para>
    /// </summary>
    /// <typeparam name="TModel">Model 类型，必须实现 INotifyPropertyChanged。</typeparam>
    /// <param name="vm">需要触发派生属性通知的 ViewModel（ObservableObject 派生类）。</param>
    /// <param name="model">被监听的 Model 实例。</param>
    /// <param name="mappings">
    /// 映射项数组。每项形如：<c>(m =&gt; m.Name, [() =&gt; Summary])</c>，
    /// 表示"当 Model.Name 变化时，触发 VM 的 Summary 派生属性通知"。
    /// </param>
    /// <returns>订阅句柄；<paramref name="model"/> 为 null 或映射为空时返回 <c>null</c>。</returns>
    public static ModelSubscription? ForwardDerived<TModel>(
        ObservableObject vm,
        TModel model,
        params (Expression<Func<TModel, object?>> ModelProp,
                Expression<Func<object>>[] DerivedProps)[] mappings)
        where TModel : INotifyPropertyChanged
    {
        if (model is null || mappings.Length == 0)
        {
            return null;
        }

        MethodInfo? raiseMethod = typeof(ObservableObject).GetMethod(
            "OnPropertyChanged",
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: [typeof(string)],
            modifiers: null);

        // 预编译映射表：Model 属性名 → 需通知的 VM 派生属性名集合
        var dispatch = new Dictionary<string, string[]>(mappings.Length);
        foreach (var (modelProp, derivedProps) in mappings)
        {
            int derivedLen = derivedProps?.Length ?? 0;
            var targets = new string[derivedLen];
            if (derivedProps is { Length: > 0 })
            {
                for (int j = 0; j < derivedProps.Length; j++)
                {
                    targets[j] = GetMemberName(derivedProps[j]);
                }
            }
            dispatch[GetMemberName(modelProp)] = targets;
        }

        void Handler(object? _, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is null || !dispatch.TryGetValue(e.PropertyName, out var targets))
            {
                return;
            }

            foreach (string name in targets)
            {
                raiseMethod?.Invoke(vm, [name]);
            }
        }

        model.PropertyChanged += Handler;
        return new ModelSubscription(model, Handler);
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

    /// <summary>
    /// 一次 <see cref="ForwardDerived{TModel}"/> 调用产生的订阅生命周期句柄。
    /// Dispose 即解绑事件订阅。
    /// <para>
    /// 典型用法：
    /// <code>
    /// _sub?.Dispose();
    /// _sub = ModelBinder.ForwardDerived(this, model, ...);
    /// </code>
    /// </para>
    /// </summary>
    public sealed class ModelSubscription : IDisposable
    {
        private INotifyPropertyChanged? _model;
        private readonly PropertyChangedEventHandler _handler;

        internal ModelSubscription(INotifyPropertyChanged model, PropertyChangedEventHandler handler)
        {
            _model = model;
            _handler = handler;
        }

        /// <summary>
        /// Unsubscribe the PropertyChanged handler. Safe to call multiple times.
        /// 解绑事件订阅；可安全重复调用。
        /// </summary>
        public void Dispose()
        {
            if (_model is not null)
            {
                _model.PropertyChanged -= _handler;
                _model = null;
            }
        }
    }
}
