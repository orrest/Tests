using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Tests.Wpf.ModelNotification.Helpers;

/// <summary>
/// Property-change monitoring helper. Only listens to changes on sources and notifies via callback;
/// does <b>not</b> invoke VM's OnPropertyChanged — that responsibility belongs to the VM.
/// 属性变化监听辅助类。只负责监听源对象/集合的属性变化并通过回调通知，
/// <b>不</b>代为调用 VM 的 OnPropertyChanged——该职责由 VM 自身承担。
/// <para>
/// 提供两个 API：
/// <list type="bullet">
/// <item><see cref="Subscribe{TSource}"/> —— 监听单个对象的指定属性变化</item>
/// <item><see cref="SubscribeCollection{TSource}"/> —— 监听集合：自动订阅每个元素的属性变化 + 集合增删</item>
/// </list>
/// </para>
/// </summary>
public static class PropertyChangeTracker
{
    /// <summary>
    /// 监听 <paramref name="source"/> 上指定的属性变化，匹配时回调 <paramref name="onChanged"/>。
    /// <para>
    /// 回调参数为发生变化的属性名（即 <paramref name="propertyNames"/> 中的一个）。
    /// VM 在回调里自行决定如何响应（通常是调用 <c>OnPropertyChanged(nameof(SomeDerived))</c>）。
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">源对象类型，必须实现 INotifyPropertyChanged。</typeparam>
    /// <param name="source">被监听的源对象（通常是下层 VM 或 Model 实例）。</param>
    /// <param name="propertyNames">关心的属性名集合（建议用 <c>nameof</c> 提供编译期检查）。</param>
    /// <param name="onChanged">属性变化时的回调，参数为变化的属性名。</param>
    /// <returns>订阅句柄；<paramref name="source"/> 为 null 时返回 <c>null</c>。</returns>
    public static ModelSubscription? Subscribe<TSource>(
        TSource source,
        IEnumerable<string> propertyNames,
        Action<string> onChanged)
        where TSource : INotifyPropertyChanged
    {
        if (source is null)
        {
            return null;
        }

        var watchSet = propertyNames as HashSet<string> ?? [.. propertyNames];

        void Handler(object? _, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not null && watchSet.Contains(e.PropertyName))
            {
                onChanged(e.PropertyName);
            }
        }

        source.PropertyChanged += Handler;
        return new ModelSubscription(() => source.PropertyChanged -= Handler);
    }

    /// <summary>
    /// 监听 <paramref name="collection"/> 中所有元素（含未来新增元素）的指定属性变化，
    /// 以及集合本身的增删/替换/重置操作。任一变化都回调 <paramref name="onChanged"/>（无参数）。
    /// <para>
    /// 自动管理元素订阅：Add 订阅新元素、Remove/Replace 解绑旧元素、Reset 全部重建。
    /// VM 在回调里自行决定如何响应（通常是 <c>OnPropertyChanged(nameof(Summary))</c>）。
    /// </para>
    /// <para>
    /// 该重载的 selector 为 identity（<c>e => e</c>），即直接监听集合元素自身的属性变化。
    /// 如需监听元素子对象（如 <c>element.Model</c>）的属性变化，使用
    /// <see cref="SubscribeCollection{TElement, TNested}(ObservableCollection{TElement}, Func{TElement, TNested}, IEnumerable{string}, Action)"/>。
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">集合元素类型，必须实现 INotifyPropertyChanged。</typeparam>
    /// <param name="collection">被监听的 ObservableCollection。</param>
    /// <param name="propertyNames">每个元素上关心的属性名集合。</param>
    /// <param name="onChanged">元素属性变化或集合增删时的回调。</param>
    /// <returns>订阅句柄；<paramref name="collection"/> 为 null 时返回 <c>null</c>。</returns>
    public static ModelSubscription? SubscribeCollection<TSource>(
        ObservableCollection<TSource> collection,
        IEnumerable<string> propertyNames,
        Action onChanged)
        where TSource : INotifyPropertyChanged
        => SubscribeCollection(collection, e => e, propertyNames, onChanged);

    /// <summary>
    /// 监听 <paramref name="collection"/> 中所有元素的<b>子对象</b>（通过 <paramref name="observableSelector"/> 提取）的指定属性变化，
    /// 以及集合本身的增删/替换/重置操作。任一变化都回调 <paramref name="onChanged"/>（无参数）。
    /// <para>
    /// 自动管理元素订阅：Add 订阅新元素（子对象）、Remove/Replace 解绑旧元素（子对象）、Reset 全部重建。
    /// </para>
    /// <para>
    /// 典型的 selector 写法：<c>ovm => ovm.Model</c>，即监听每个 VM 所持有 Model 的属性变化。
    /// 这样 VM 无需代理属性即可感知下层 Model 变化，直接刷新自身派生属性。
    /// </para>
    /// </summary>
    /// <typeparam name="TElement">集合元素类型（无需 INotifyPropertyChanged 约束）。</typeparam>
    /// <typeparam name="TNested">子对象类型，必须实现 INotifyPropertyChanged。</typeparam>
    /// <param name="collection">被监听的 ObservableCollection。</param>
    /// <param name="observableSelector">从元素提取子对象的委托。</param>
    /// <param name="propertyNames">每个子对象上关心的属性名集合。</param>
    /// <param name="onChanged">元素属性变化或集合增删时的回调。</param>
    /// <returns>订阅句柄；<paramref name="collection"/> 为 null 时返回 <c>null</c>。</returns>
    public static ModelSubscription? SubscribeCollection<TElement, TNested>(
        ObservableCollection<TElement> collection,
        Func<TElement, TNested> observableSelector,
        IEnumerable<string> propertyNames,
        Action onChanged)
        where TElement : notnull
        where TNested : INotifyPropertyChanged
    {
        if (collection is null)
        {
            return null;
        }

        var watchSet = propertyNames as HashSet<string> ?? [.. propertyNames];
        var elementSubs = new Dictionary<TElement, ModelSubscription>();

        // 为单个元素提取子对象并建立属性订阅
        ModelSubscription SubscribeElement(TElement element)
        {
            var nested = observableSelector(element);
            if (nested is null)
            {
                return new ModelSubscription(() => { });
            }
            return Subscribe(nested, watchSet, _ => onChanged())!;
        }

        // 解绑单个元素订阅
        void UnsubscribeElement(TElement element)
        {
            if (elementSubs.Remove(element, out var sub))
            {
                sub.Dispose();
            }
        }

        // 初始化：订阅所有现有元素
        foreach (TElement element in collection)
        {
            elementSubs[element] = SubscribeElement(element);
        }

        // 订阅集合变化：动态增删元素订阅 + 触发回调
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
                    onChanged();
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems is not null)
                    {
                        foreach (TElement item in e.OldItems)
                        {
                            UnsubscribeElement(item);
                        }
                    }
                    onChanged();
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
                    onChanged();
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
                    onChanged();
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
