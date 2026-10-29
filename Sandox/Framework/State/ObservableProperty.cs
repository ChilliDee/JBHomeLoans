namespace Mortgage.State;

// Local Sandox version of the CRM framework class of the same name (no BusinessLogic.Core package).
// A value on an _S class that components can subscribe to. Setting Value notifies every subscriber.

public class ConditionalSubscriber<T>
{
    public Action<T, StateChangeContext> Callback { get; set; } = default!;
    public T TargetValue { get; set; } = default!;
    public string SubscribedFrom { get; set; } = string.Empty;
}

public class ConditionalAsyncSubscriber<T>
{
    public Func<T, StateChangeContext, Task> Callback { get; set; } = default!;
    public T TargetValue { get; set; } = default!;
    public string SubscribedFrom { get; set; } = string.Empty;
}

public class ConditionalAsyncSubscriberNoData<T>
{
    public Func<StateChangeContext, Task> Callback { get; set; } = default!;
    public T TargetValue { get; set; } = default!;
    public string SubscribedFrom { get; set; } = string.Empty;
}

public class ObservableProperty<T>
{
    private CancellationTokenSource? _debounceCancellationTokenSource;
    private T _value;

    private readonly List<(Action<T, StateChangeContext> Callback, string SubscribedFrom)> _subscribers = new();
    private readonly List<(Func<T, StateChangeContext, Task> Callback, string SubscribedFrom)> _asyncSubscribers = new();
    private readonly List<(Func<StateChangeContext, Task> Callback, string SubscribedFrom)> _asyncSubscribersNoData = new();
    private readonly List<ConditionalSubscriber<T>> _conditionalSubscribers = new();
    private readonly List<ConditionalAsyncSubscriber<T>> _conditionalAsyncSubscribers = new();
    private readonly List<ConditionalAsyncSubscriberNoData<T>> _conditionalAsyncSubscribersNoData = new();
    private readonly List<ConditionalSubscriber<T>> _inverseConditionalSubscribers = new();
    private readonly List<ConditionalAsyncSubscriber<T>> _inverseConditionalAsyncSubscribers = new();
    private readonly List<ConditionalAsyncSubscriberNoData<T>> _inverseConditionalAsyncSubscribersNoData = new();

    private StateChangeContext _context;

    public ObservableProperty(string propertyName, T initialValue, string componentName)
    {
        PropertyName = propertyName;
        ComponentName = componentName;
        _value = initialValue;
        _context = new StateChangeContext(componentName);
    }

    public string PropertyName { get; private set; }
    public string ComponentName { get; private set; }

    // Names of everything currently subscribed (the subscribedFrom argument of each subscribe call)
    public List<string> Subscribers =>
        _subscribers.Select(s => s.SubscribedFrom)
            .Concat(_asyncSubscribers.Select(s => s.SubscribedFrom))
            .Concat(_asyncSubscribersNoData.Select(s => s.SubscribedFrom))
            .Concat(_conditionalSubscribers.Select(s => s.SubscribedFrom))
            .Concat(_conditionalAsyncSubscribers.Select(s => s.SubscribedFrom))
            .Concat(_conditionalAsyncSubscribersNoData.Select(s => s.SubscribedFrom))
            .Concat(_inverseConditionalSubscribers.Select(s => s.SubscribedFrom))
            .Concat(_inverseConditionalAsyncSubscribers.Select(s => s.SubscribedFrom))
            .Concat(_inverseConditionalAsyncSubscribersNoData.Select(s => s.SubscribedFrom))
            .ToList();

    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            _context = new StateChangeContext(ComponentName);
            _context.AddToChain(PropertyName);
            NotifySubscribers(value);
        }
    }

    public async Task SetValueDebounced(T newValue, StateChangeContext context, int delayMs = 1500)
    {
        _debounceCancellationTokenSource?.Cancel();
        _debounceCancellationTokenSource = new CancellationTokenSource();

        try
        {
            await Task.Delay(delayMs, _debounceCancellationTokenSource.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        _value = newValue;
        _context = context;
        _context.AddToChain(PropertyName);
        NotifySubscribers(newValue);
    }

    // NOTIFY =====================================================================================================================
    private void NotifySubscribers(T newValue)
    {
        NotifyNonAsyncSubscribers(newValue);
        NotifyConditionalSubscribers(newValue);
        NotifyInverseConditionalSubscribers(newValue);

        // The Value setter is synchronous, so async subscribers are started and observed, not awaited.
        Observe(NotifyAsyncSubscribers(newValue));
        Observe(NotifyAsyncSubscribersNoData());
        Observe(NotifyConditionalAsyncSubscribers(newValue));
        Observe(NotifyConditionalAsyncSubscribersNoData(newValue));
        Observe(NotifyInverseConditionalAsyncSubscribers(newValue));
        Observe(NotifyInverseConditionalAsyncSubscribersNoData(newValue));
    }

    private void Observe(Task task)
    {
        task.ContinueWith(
            t => Console.Error.WriteLine($"[ObservableProperty] {ComponentName}.{PropertyName} subscriber failed: {t.Exception?.GetBaseException().Message}"),
            TaskContinuationOptions.OnlyOnFaulted);
    }

    private static bool Matches(T value, T targetValue) => EqualityComparer<T>.Default.Equals(value, targetValue);

    private void NotifyNonAsyncSubscribers(T newValue)
    {
        foreach (var subscriber in _subscribers.ToList())
        {
            subscriber.Callback(newValue, _context);
        }
    }

    private async Task NotifyAsyncSubscribers(T newValue)
    {
        foreach (var subscriber in _asyncSubscribers.ToList())
        {
            await subscriber.Callback(newValue, _context);
        }
    }

    private async Task NotifyAsyncSubscribersNoData()
    {
        foreach (var subscriber in _asyncSubscribersNoData.ToList())
        {
            await subscriber.Callback(_context);
        }
    }

    private void NotifyConditionalSubscribers(T newValue)
    {
        foreach (var subscriber in _conditionalSubscribers.Where(s => Matches(newValue, s.TargetValue)).ToList())
        {
            subscriber.Callback(newValue, _context);
        }
    }

    private async Task NotifyConditionalAsyncSubscribers(T newValue)
    {
        foreach (var subscriber in _conditionalAsyncSubscribers.Where(s => Matches(newValue, s.TargetValue)).ToList())
        {
            await subscriber.Callback(newValue, _context);
        }
    }

    private async Task NotifyConditionalAsyncSubscribersNoData(T newValue)
    {
        foreach (var subscriber in _conditionalAsyncSubscribersNoData.Where(s => Matches(newValue, s.TargetValue)).ToList())
        {
            await subscriber.Callback(_context);
        }
    }

    private void NotifyInverseConditionalSubscribers(T newValue)
    {
        foreach (var subscriber in _inverseConditionalSubscribers.Where(s => !Matches(newValue, s.TargetValue)).ToList())
        {
            subscriber.Callback(newValue, _context);
        }
    }

    private async Task NotifyInverseConditionalAsyncSubscribers(T newValue)
    {
        foreach (var subscriber in _inverseConditionalAsyncSubscribers.Where(s => !Matches(newValue, s.TargetValue)).ToList())
        {
            await subscriber.Callback(newValue, _context);
        }
    }

    private async Task NotifyInverseConditionalAsyncSubscribersNoData(T newValue)
    {
        foreach (var subscriber in _inverseConditionalAsyncSubscribersNoData.Where(s => !Matches(newValue, s.TargetValue)).ToList())
        {
            await subscriber.Callback(_context);
        }
    }

    // SUBSCRIBE ==================================================================================================================
    public void Subscribe(Action<T, StateChangeContext> subscriber, string subscribedFrom)
        => _subscribers.Add((subscriber, subscribedFrom));

    public void SubscribeAsync(Func<T, StateChangeContext, Task> asyncSubscriber, string subscribedFrom)
        => _asyncSubscribers.Add((asyncSubscriber, subscribedFrom));

    public void NoDataSubscribeAsync(Func<StateChangeContext, Task> subscriber, string subscribedFrom)
        => _asyncSubscribersNoData.Add((subscriber, subscribedFrom));

    public void SubscribeConditional(Action<T, StateChangeContext> subscriber, T targetValue, string subscribedFrom)
        => _conditionalSubscribers.Add(new() { Callback = subscriber, TargetValue = targetValue, SubscribedFrom = subscribedFrom });

    public void SubscribeConditionalAsync(Func<T, StateChangeContext, Task> asyncSubscriber, T targetValue, string subscribedFrom)
        => _conditionalAsyncSubscribers.Add(new() { Callback = asyncSubscriber, TargetValue = targetValue, SubscribedFrom = subscribedFrom });

    public void ConditionalNoDataSubscribeAsync(Func<StateChangeContext, Task> subscriber, T targetValue, string subscribedFrom)
        => _conditionalAsyncSubscribersNoData.Add(new() { Callback = subscriber, TargetValue = targetValue, SubscribedFrom = subscribedFrom });

    public void SubscribeInverseConditional(Action<T, StateChangeContext> subscriber, T targetValue, string subscribedFrom)
        => _inverseConditionalSubscribers.Add(new() { Callback = subscriber, TargetValue = targetValue, SubscribedFrom = subscribedFrom });

    public void SubscribeInverseConditionalAsync(Func<T, StateChangeContext, Task> asyncSubscriber, T targetValue, string subscribedFrom)
        => _inverseConditionalAsyncSubscribers.Add(new() { Callback = asyncSubscriber, TargetValue = targetValue, SubscribedFrom = subscribedFrom });

    public void SubscribeInverseConditionalAsyncNoData(Func<StateChangeContext, Task> subscriber, T targetValue, string subscribedFrom)
        => _inverseConditionalAsyncSubscribersNoData.Add(new() { Callback = subscriber, TargetValue = targetValue, SubscribedFrom = subscribedFrom });

    // UNSUBSCRIBE ================================================================================================================
    public void Unsubscribe(Action<T, StateChangeContext> subscriber)
        => _subscribers.RemoveAll(s => s.Callback == subscriber);

    public void AsyncUnsubscribe(Func<T, StateChangeContext, Task> subscriber)
        => _asyncSubscribers.RemoveAll(s => s.Callback == subscriber);

    public void NoDataUnSubscribeAsync(Func<StateChangeContext, Task> subscriber)
        => _asyncSubscribersNoData.RemoveAll(s => s.Callback == subscriber);

    public void ConditionalUnsubscribe(Action<T, StateChangeContext> subscriber, T targetValue)
        => _conditionalSubscribers.RemoveAll(s => s.Callback == subscriber && Matches(s.TargetValue, targetValue));

    public void ConditionalAsyncUnsubscribe(Func<T, StateChangeContext, Task> subscriber, T targetValue)
        => _conditionalAsyncSubscribers.RemoveAll(s => s.Callback == subscriber && Matches(s.TargetValue, targetValue));

    public void ConditionalNoDataUnSubscribeAsync(Func<StateChangeContext, Task> subscriber, T targetValue)
        => _conditionalAsyncSubscribersNoData.RemoveAll(s => s.Callback == subscriber && Matches(s.TargetValue, targetValue));

    public void InverseConditionalUnsubscribe(Action<T, StateChangeContext> subscriber, T targetValue)
        => _inverseConditionalSubscribers.RemoveAll(s => s.Callback == subscriber && Matches(s.TargetValue, targetValue));

    public void InverseConditionalAsyncUnsubscribe(Func<T, StateChangeContext, Task> subscriber, T targetValue)
        => _inverseConditionalAsyncSubscribers.RemoveAll(s => s.Callback == subscriber && Matches(s.TargetValue, targetValue));

    public void InverseConditionalAsyncUnsubscribeNoData(Func<StateChangeContext, Task> subscriber, T targetValue)
        => _inverseConditionalAsyncSubscribersNoData.RemoveAll(s => s.Callback == subscriber && Matches(s.TargetValue, targetValue));

    // RE-CHECK ===================================================================================================================
    // Fires the conditional subscribers for targetValue if the current value already equals it (no value change needed).
    public void CheckAndNotifyConditional(T targetValue)
    {
        if (!Matches(_value, targetValue))
        {
            return;
        }

        foreach (var subscriber in _conditionalSubscribers.Where(s => Matches(s.TargetValue, targetValue)).ToList())
        {
            subscriber.Callback(_value, _context);
        }

        Observe(NotifyConditionalAsyncSubscribersForTarget(targetValue));
    }

    // Fires the inverse conditional subscribers for targetValue if the current value is already different from it.
    public void CheckAndNotifyInverseConditional(T targetValue)
    {
        if (Matches(_value, targetValue))
        {
            return;
        }

        foreach (var subscriber in _inverseConditionalSubscribers.Where(s => Matches(s.TargetValue, targetValue)).ToList())
        {
            subscriber.Callback(_value, _context);
        }

        Observe(NotifyInverseConditionalAsyncSubscribersForTarget(targetValue));
    }

    private async Task NotifyConditionalAsyncSubscribersForTarget(T targetValue)
    {
        foreach (var subscriber in _conditionalAsyncSubscribers.Where(s => Matches(s.TargetValue, targetValue)).ToList())
        {
            await subscriber.Callback(_value, _context);
        }

        foreach (var subscriber in _conditionalAsyncSubscribersNoData.Where(s => Matches(s.TargetValue, targetValue)).ToList())
        {
            await subscriber.Callback(_context);
        }
    }

    private async Task NotifyInverseConditionalAsyncSubscribersForTarget(T targetValue)
    {
        foreach (var subscriber in _inverseConditionalAsyncSubscribers.Where(s => Matches(s.TargetValue, targetValue)).ToList())
        {
            await subscriber.Callback(_value, _context);
        }

        foreach (var subscriber in _inverseConditionalAsyncSubscribersNoData.Where(s => Matches(s.TargetValue, targetValue)).ToList())
        {
            await subscriber.Callback(_context);
        }
    }
}
