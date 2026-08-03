using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Tests.Wpf.ModelNotification.Helpers;

/// <summary>
/// Property-change monitoring helper. Listens to changes on a source and, on match, reflection-invokes
/// <c>OnPropertyChanged(propertyName)</c> on a target — so the target's derived properties refresh automatically.
/// The target never needs to expose a <c>Refresh</c> method; the helper bypasses the <c>protected</c>
/// accessibility of <c>OnPropertyChanged</c> via a delegate compiled at subscription time.
/// 属性变化监听辅助类。监听源对象/集合的属性变化，匹配时通过反射在 target 上调用
/// <c>OnPropertyChanged(propertyName)</c>，使 target 的派生属性自动刷新。
/// target 无需暴露 <c>Refresh</c> 方法；辅助类在订阅时编译委托，绕过 <c>OnPropertyChanged</c> 的 <c>protected</c> 可见性。
/// <para>
/// 提供 API：
/// <list type="bullet">
/// <item><see cref="Subscribe{TSource, TTarget}"/> —— 监听单个源对象的指定属性变化，反射触发 target 的派生属性通知</item>
/// <item><see cref="SubscribeCollection{TElement, TNested, TTarget}"/> —— 监听集合（含增删）：每个元素的子对象属性变化或集合变动，反射触发 target 的派生属性通知</item>
/// </list>
/// </para>
/// <para>
/// <b>要求</b>：target 必须暴露 <c>OnPropertyChanged(string)</c> 方法（如继承自
/// <c>CommunityToolkit.Mvvm.ComponentModel.ObservableObject</c>）。否则抛 <see cref="InvalidOperationException"/>。
/// </para>
/// <para>
/// <b>性能</b>：反射委托在每个订阅建立时通过 <see cref="Expression{TDelegate}.Compile"/> 编译一次，
/// 后续回调开销接近直接方法调用；不使用全局缓存，无内存泄漏风险。
/// </para>
/// </summary>
public static class PropertyChangeTracker
{
    /// <summary>
    /// 监听 <paramref name="source"/> 上指定的属性变化，匹配时通过反射触发 <paramref name="target"/>
    /// 上的 <c>OnPropertyChanged(propertyName)</c>，使 target 的派生属性刷新。
    /// 典型场景：父 VM 装配自身/兄弟 VM/下层 Model 之间的依赖——target 无需暴露任何 <c>Refresh</c> 方法。
    /// Observe specified properties on <paramref name="source"/>; on match, reflection-invoke
    /// <c>OnPropertyChanged(propertyName)</c> on <paramref name="target"/> so its derived properties refresh.
    /// Typical for parent-VM orchestration of self/sibling/child-VM dependencies — target needs no <c>Refresh</c>.
    /// </summary>
    /// <typeparam name="TSource">源对象类型，必须实现 INotifyPropertyChanged。</typeparam>
    /// <typeparam name="TTarget">目标对象类型，必须实现 INotifyPropertyChanged 且暴露 OnPropertyChanged(string)。</typeparam>
    /// <param name="source">被监听的源对象（通常是下层 VM、Model 或自身）。</param>
    /// <param name="sourcePropertyNames">关心的源属性名集合（建议用 <c>nameof</c> 提供编译期检查）。</param>
    /// <param name="target">属性变化时要触发通知的目标对象（通常是父 VM 自身或兄弟 VM）。</param>
    /// <param name="targetPropertyNames">每次源变化时在 target 上引发的属性名集合（通常为 target 的派生属性）。</param>
    /// <returns>订阅句柄；<paramref name="source"/> 为 null 时返回 <c>null</c>。</returns>
    /// <exception cref="InvalidOperationException">target 未暴露 <c>OnPropertyChanged(string)</c> 方法。</exception>
    public static ModelSubscription? Subscribe<TSource, TTarget>(
        TSource source,
        IEnumerable<string> sourcePropertyNames,
        TTarget target,
        IEnumerable<string> targetPropertyNames)
        where TSource : INotifyPropertyChanged
        where TTarget : INotifyPropertyChanged
    {
        if (source is null)
        {
            return null;
        }

        var watchSet = sourcePropertyNames as HashSet<string> ?? [.. sourcePropertyNames];

        // 订阅时一次性编译所有 target raiser，回调里直接调用——无全局缓存
        var raisers = targetPropertyNames
            .Select(p => BuildRaiser(target, p))
            .ToList();

        void Handler(object? _, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not null && watchSet.Contains(e.PropertyName))
            {
                foreach (var raise in raisers)
                {
                    raise();
                }
            }
        }

        source.PropertyChanged += Handler;
        return new ModelSubscription(() => source.PropertyChanged -= Handler);
    }

    /// <summary>
    /// 监听 <paramref name="collection"/> 中所有元素（含未来新增元素）的<b>子对象</b>（通过
    /// <paramref name="observableSelector"/> 提取）的指定属性变化，以及集合本身的增删/替换/重置操作。
    /// 任一变化都通过反射触发 <paramref name="target"/> 上的 <c>OnPropertyChanged(propertyName)</c>。
    /// Observe each element's nested object (via <paramref name="observableSelector"/>) for specified property changes,
    /// plus the collection's add/remove/replace/reset. Any change reflection-invokes OnPropertyChanged on <paramref name="target"/>.
    /// <para>
    /// 自动管理元素订阅：Add 订阅新元素（子对象）、Remove/Replace 解绑旧元素（子对象）、Reset 全部重建。
    /// </para>
    /// <para>
    /// 典型 selector 写法：
    /// <list type="bullet">
    /// <item><c>e => e</c>（identity）—— 直接监听元素自身的属性变化（元素本身需实现 INotifyPropertyChanged）</item>
    /// <item><c>ovm => ovm.Model</c> —— 监听元素所持有 Model 的属性变化</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <typeparam name="TElement">集合元素类型（无需 INotifyPropertyChanged 约束）。</typeparam>
    /// <typeparam name="TNested">子对象类型，必须实现 INotifyPropertyChanged。</typeparam>
    /// <typeparam name="TTarget">目标对象类型，必须实现 INotifyPropertyChanged 且暴露 OnPropertyChanged(string)。</typeparam>
    /// <param name="collection">被监听的 ObservableCollection。</param>
    /// <param name="observableSelector">从元素提取子对象的委托（identity 时传 <c>e => e</c>）。</param>
    /// <param name="sourcePropertyNames">每个子对象上关心的属性名集合。</param>
    /// <param name="target">元素属性变化或集合增删时要触发通知的目标对象（通常是父 VM 自身）。</param>
    /// <param name="targetPropertyNames">每次变化时在 target 上引发的属性名集合（通常为 target 的派生属性）。</param>
    /// <returns>订阅句柄；<paramref name="collection"/> 为 null 时返回 <c>null</c>。</returns>
    /// <exception cref="InvalidOperationException">target 未暴露 <c>OnPropertyChanged(string)</c> 方法。</exception>
    public static ModelSubscription? SubscribeCollection<TElement, TNested, TTarget>(
        ObservableCollection<TElement> collection,
        Func<TElement, TNested> observableSelector,
        IEnumerable<string> sourcePropertyNames,
        TTarget target,
        IEnumerable<string> targetPropertyNames)
        where TElement : notnull
        where TNested : INotifyPropertyChanged
        where TTarget : INotifyPropertyChanged
    {
        if (collection is null)
        {
            return null;
        }

        var watchSet = sourcePropertyNames as HashSet<string> ?? [.. sourcePropertyNames];
        var elementSubs = new Dictionary<TElement, ModelSubscription>();

        // 为单个元素提取子对象并建立属性订阅（复用单源 Subscribe 反射版）
        ModelSubscription SubscribeElement(TElement element)
        {
            var nested = observableSelector(element);
            if (nested is null)
            {
                return new ModelSubscription(() => { });
            }
            return Subscribe(nested, watchSet, target, targetPropertyNames)!;
        }

        // 解绑单个元素订阅
        void UnsubscribeElement(TElement element)
        {
            if (elementSubs.Remove(element, out var sub))
            {
                sub.Dispose();
            }
        }

        // 订阅时一次性编译所有 target raiser，供集合变化回调（Add/Remove/Reset）直接复用
        var raisers = targetPropertyNames
            .Select(p => BuildRaiser(target, p))
            .ToList();

        void RaiseTargets()
        {
            foreach (var raise in raisers)
            {
                raise();
            }
        }

        // 初始化：订阅所有现有元素
        foreach (TElement element in collection)
        {
            elementSubs[element] = SubscribeElement(element);
        }

        // 订阅集合变化：动态增删元素订阅 + 触发 target 通知
        void CollectionHandler(object? _, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems is not null)
                    {
                        foreach (TElement item in e.NewItems)
                        {
                            elementSubs[item] = SubscribeElement(item);
                        }
                    }
                    RaiseTargets();
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems is not null)
                    {
                        foreach (TElement item in e.OldItems)
                        {
                            UnsubscribeElement(item);
                        }
                    }
                    RaiseTargets();
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems is not null)
                    {
                        foreach (TElement item in e.OldItems)
                        {
                            UnsubscribeElement(item);
                        }
                    }
                    if (e.NewItems is not null)
                    {
                        foreach (TElement item in e.NewItems)
                        {
                            elementSubs[item] = SubscribeElement(item);
                        }
                    }
                    RaiseTargets();
                    break;

                case NotifyCollectionChangedAction.Reset:
                    foreach (var sub in elementSubs.Values)
                    {
                        sub.Dispose();
                    }
                    elementSubs.Clear();
                    foreach (TElement element in collection)
                    {
                        elementSubs[element] = SubscribeElement(element);
                    }
                    RaiseTargets();
                    break;
            }
        }

        collection.CollectionChanged += CollectionHandler;

        return new ModelSubscription(() =>
        {
            collection.CollectionChanged -= CollectionHandler;
            foreach (var sub in elementSubs.Values)
            {
                sub.Dispose();
            }
            elementSubs.Clear();
        });
    }

    /// <summary>
    /// Build a no-arg delegate that invokes <c>OnPropertyChanged(propertyName)</c> on <paramref name="target"/>
    /// via reflection. Compiled once per subscription via <see cref="Expression{TDelegate}.Compile"/>,
    /// so per-call cost is close to a direct method call. Not cached globally.
    /// 通过反射构造一个无参委托，调用 <paramref name="target"/> 上的 <c>OnPropertyChanged(propertyName)</c>。
    /// 每次订阅时编译一次（<see cref="Expression{TDelegate}.Compile"/>），单次调用开销接近直接方法调用；不做全局缓存。
    /// </summary>
    /// <exception cref="InvalidOperationException">target 未暴露 <c>OnPropertyChanged(string)</c> 方法。</exception>
    private static Action BuildRaiser(INotifyPropertyChanged target, string propertyName)
    {
        var method = target.GetType().GetMethod(
            "OnPropertyChanged",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
            binder: null,
            types: [typeof(string)],
            modifiers: null);

        if (method is null)
        {
            throw new InvalidOperationException(
                $"{target.GetType().FullName} does not expose OnPropertyChanged(string) — " +
                "cannot use PropertyChangeTracker. " +
                "Inherit from ObservableObject (or expose a matching protected/public method).");
        }

        var targetParam = Expression.Parameter(typeof(object));
        var call = Expression.Call(
            Expression.Convert(targetParam, target.GetType()),
            method,
            Expression.Constant(propertyName, typeof(string)));
        var lambda = Expression.Lambda<Action<object>>(call, targetParam);
        var compiled = lambda.Compile();
        return () => compiled(target);
    }

    /// <summary>
    /// 订阅生命周期句柄。Dispose 即执行清理回调（解绑事件订阅）；可安全重复调用。
    /// </summary>
    public sealed class ModelSubscription : IDisposable
    {
        private Action? _cleanup;

        internal ModelSubscription(Action cleanup)
        {
            _cleanup = cleanup;
        }

        public void Dispose()
        {
            _cleanup?.Invoke();
            _cleanup = null;
        }
    }
}
