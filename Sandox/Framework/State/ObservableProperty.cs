using System.Linq;

namespace Mortgage.State
{
    public class ObservableProperty<T>
    {
        private CancellationTokenSource? _debounceCancellationTokenSource;
        public string PropertyName { get; private set; }
        public string ComponentName { get; private set; }

        // All subscriber names in one place — hover over the observable property to see who's listening
        public List<string> Subscribers { get; } = new();

        private T _value;

        private List<(Action<T, StateChangeContext> Callback, string SubscribedFrom)> _subscribers;
        private List<(Func<T, StateChangeContext, Task> Callback, string SubscribedFrom)> _asyncSubscribers;
        private List<(Func<StateChangeContext, Task> Callback, string SubscribedFrom)> _asyncSubscribersNoData;

        private List<ConditionalSubscriber<T>> _conditionalSubscribers;
        private List<ConditionalAsyncSubscriber<T>> _conditionalAsyncSubscribers;
        private List<ConditionalAsyncSubscriberNoData<T>> _conditionalAsyncSubscribersNoData;

        private List<ConditionalSubscriber<T>> _inverseConditionalSubscribers;
        private List<ConditionalAsyncSubscriber<T>> _inverseConditionalAsyncSubscribers;
        private List<ConditionalAsyncSubscriberNoData<T>> _inverseConditionalAsyncSubscribersNoData;

        private StateChangeContext _context;

        public ObservableProperty(string propertyName, T initialValue, string componentName)
        {
            PropertyName = propertyName;
            ComponentName = componentName;
            _value = initialValue;
            _subscribers = new();
            _asyncSubscribers = new();
            _asyncSubscribersNoData = new();
            _conditionalSubscribers = new();
            _conditionalAsyncSubscribers = new();
            _conditionalAsyncSubscribersNoData = new();
            _inverseConditionalSubscribers = new();
            _inverseConditionalAsyncSubscribers = new();
            _inverseConditionalAsyncSubscribersNoData = new();
        }

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
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
                NotifySubscribers(newValue);
            }
            catch (TaskCanceledException)
            {
                // Expected when cancelled by a new call
            }
        }

        private void NotifySubscribers(T newValue)
        {
            if (_subscribers.Count > 0)
                NotifyNonAsyncSubscribers(newValue);

            if (_asyncSubscribers.Count > 0)
                _ = NotifyAsyncSubscribers(newValue);

            if (_asyncSubscribersNoData.Count > 0)
                _ = NotifyAsyncSubscribersNoData();

            if (_conditionalSubscribers.Count > 0)
                NotifyConditionalSubscribers(newValue);

            if (_conditionalAsyncSubscribers.Count > 0)
                _ = NotifyConditionalAsyncSubscribers(newValue);

            if (_conditionalAsyncSubscribersNoData.Count > 0)
                _ = NotifyConditionalAsyncSubscribersNoData(newValue);

            if (_inverseConditionalSubscribers.Count > 0)
                NotifyInverseConditionalSubscribers(newValue);

            if (_inverseConditionalAsyncSubscribers.Count > 0)
                _ = NotifyInverseConditionalAsyncSubscribers(newValue);

            if (_inverseConditionalAsyncSubscribersNoData.Count > 0)
                _ = NotifyInverseConditionalAsyncSubscribersNoData(newValue);
        }

        private void NotifyNonAsyncSubscribers(T newValue)
        {
            foreach (var (callback, _) in _subscribers)
                callback(newValue, _context);
        }

        private async Task NotifyAsyncSubscribers(T newValue)
        {
            foreach (var (callback, _) in _asyncSubscribers)
                await callback(newValue, _context);
        }

        private async Task NotifyAsyncSubscribersNoData()
        {
            foreach (var (callback, _) in _asyncSubscribersNoData)
                await callback(_context);
        }

        private void NotifyConditionalSubscribers(T newValue)
        {
            foreach (var subscriber in _conditionalSubscribers)
            {
                if (EqualityComparer<T>.Default.Equals(newValue, subscriber.TargetValue))
                    subscriber.Callback(newValue, _context);
            }
        }

        private async Task NotifyConditionalAsyncSubscribers(T newValue)
        {
            foreach (var subscriber in _conditionalAsyncSubscribers)
            {
                if (EqualityComparer<T>.Default.Equals(newValue, subscriber.TargetValue))
                    await subscriber.Callback(newValue, _context);
            }
        }

        private async Task NotifyConditionalAsyncSubscribersNoData(T newValue)
        {
            foreach (var subscriber in _conditionalAsyncSubscribersNoData)
            {
                if (EqualityComparer<T>.Default.Equals(newValue, subscriber.TargetValue))
                    await subscriber.Callback(_context);
            }
        }

        private void NotifyInverseConditionalSubscribers(T newValue)
        {
            foreach (var subscriber in _inverseConditionalSubscribers)
            {
                if (!EqualityComparer<T>.Default.Equals(newValue, subscriber.TargetValue))
                    subscriber.Callback(newValue, _context);
            }
        }

        private async Task NotifyInverseConditionalAsyncSubscribers(T newValue)
        {
            foreach (var subscriber in _inverseConditionalAsyncSubscribers)
            {
                if (!EqualityComparer<T>.Default.Equals(newValue, subscriber.TargetValue))
                    await subscriber.Callback(newValue, _context);
            }
        }

        private async Task NotifyInverseConditionalAsyncSubscribersNoData(T newValue)
        {
            foreach (var subscriber in _inverseConditionalAsyncSubscribersNoData)
            {
                if (!EqualityComparer<T>.Default.Equals(newValue, subscriber.TargetValue))
                    await subscriber.Callback(_context);
            }
        }

        // -------------------------
        // Subscribe methods
        // -------------------------

        public void Subscribe(Action<T, StateChangeContext> subscriber, string subscribedFrom)
        {
            _subscribers.Add((subscriber, subscribedFrom));
            Subscribers.Add(subscribedFrom);
        }

        public void SubscribeAsync(Func<T, StateChangeContext, Task> asyncSubscriber, string subscribedFrom)
        {
            _asyncSubscribers.Add((asyncSubscriber, subscribedFrom));
            Subscribers.Add(subscribedFrom);
        }

        public void NoDataSubscribeAsync(Func<StateChangeContext, Task> subscriber, string subscribedFrom)
        {
            _asyncSubscribersNoData.Add((subscriber, subscribedFrom));
            Subscribers.Add(subscribedFrom);
        }

        public void SubscribeConditional(Action<T, StateChangeContext> subscriber, T targetValue, string subscribedFrom)
        {
            _conditionalSubscribers.Add(new ConditionalSubscriber<T>
            {
                Callback = subscriber,
                TargetValue = targetValue,
                SubscribedFrom = subscribedFrom
            });
            Subscribers.Add(subscribedFrom);
        }

        public void SubscribeConditionalAsync(Func<T, StateChangeContext, Task> asyncSubscriber, T targetValue, string subscribedFrom)
        {
            _conditionalAsyncSubscribers.Add(new ConditionalAsyncSubscriber<T>
            {
                Callback = asyncSubscriber,
                TargetValue = targetValue,
                SubscribedFrom = subscribedFrom
            });
            Subscribers.Add(subscribedFrom);
        }

        public void ConditionalNoDataSubscribeAsync(Func<StateChangeContext, Task> subscriber, T targetValue, string subscribedFrom)
        {
            _conditionalAsyncSubscribersNoData.Add(new ConditionalAsyncSubscriberNoData<T>
            {
                Callback = subscriber,
                TargetValue = targetValue,
                SubscribedFrom = subscribedFrom
            });
            Subscribers.Add(subscribedFrom);
        }

        public void SubscribeInverseConditional(Action<T, StateChangeContext> subscriber, T targetValue, string subscribedFrom)
        {
            _inverseConditionalSubscribers.Add(new ConditionalSubscriber<T>
            {
                Callback = subscriber,
                TargetValue = targetValue,
                SubscribedFrom = subscribedFrom
            });
            Subscribers.Add(subscribedFrom);
        }

        public void SubscribeInverseConditionalAsync(Func<T, StateChangeContext, Task> asyncSubscriber, T targetValue, string subscribedFrom)
        {
            _inverseConditionalAsyncSubscribers.Add(new ConditionalAsyncSubscriber<T>
            {
                Callback = asyncSubscriber,
                TargetValue = targetValue,
                SubscribedFrom = subscribedFrom
            });
            Subscribers.Add(subscribedFrom);
        }

        public void SubscribeInverseConditionalAsyncNoData(Func<StateChangeContext, Task> subscriber, T targetValue, string subscribedFrom)
        {
            _inverseConditionalAsyncSubscribersNoData.Add(new ConditionalAsyncSubscriberNoData<T>
            {
                Callback = subscriber,
                TargetValue = targetValue,
                SubscribedFrom = subscribedFrom
            });
            Subscribers.Add(subscribedFrom);
        }

        // -------------------------
        // Unsubscribe methods
        // -------------------------

        public void Unsubscribe(Action<T, StateChangeContext> subscriber)
        {
            var match = _subscribers.FirstOrDefault(s => s.Callback.Method == subscriber.Method && s.Callback.Target == subscriber.Target);
            if (match != default) Subscribers.Remove(match.SubscribedFrom);
            _subscribers.RemoveAll(s => s.Callback.Method == subscriber.Method && s.Callback.Target == subscriber.Target);
        }

        public void AsyncUnsubscribe(Func<T, StateChangeContext, Task> subscriber)
        {
            var match = _asyncSubscribers.FirstOrDefault(s => s.Callback.Method == subscriber.Method && s.Callback.Target == subscriber.Target);
            if (match != default) Subscribers.Remove(match.SubscribedFrom);
            _asyncSubscribers.RemoveAll(s => s.Callback.Method == subscriber.Method && s.Callback.Target == subscriber.Target);
        }

        public void NoDataUnSubscribeAsync(Func<StateChangeContext, Task> subscriber)
        {
            var match = _asyncSubscribersNoData.FirstOrDefault(s => s.Callback.Method == subscriber.Method && s.Callback.Target == subscriber.Target);
            if (match != default) Subscribers.Remove(match.SubscribedFrom);
            _asyncSubscribersNoData.RemoveAll(s => s.Callback.Method == subscriber.Method && s.Callback.Target == subscriber.Target);
        }

        public void ConditionalUnsubscribe(Action<T, StateChangeContext> subscriber, T targetValue)
        {
            var match = _conditionalSubscribers.FirstOrDefault(s =>
                s.Callback.Method == subscriber.Method &&
                s.Callback.Target == subscriber.Target &&
                EqualityComparer<T>.Default.Equals(s.TargetValue, targetValue));
            if (match != null) Subscribers.Remove(match.SubscribedFrom);
            _conditionalSubscribers.RemoveAll(s =>
                s.Callback.Method == subscriber.Method &&
                s.Callback.Target == subscriber.Target &&
                EqualityComparer<T>.Default.Equals(s.TargetValue, targetValue));
        }

        public void ConditionalAsyncUnsubscribe(Func<T, StateChangeContext, Task> subscriber, T targetValue)
        {
            var match = _conditionalAsyncSubscribers.FirstOrDefault(s =>
                s.Callback.Method == subscriber.Method &&
                s.Callback.Target == subscriber.Target &&
                EqualityComparer<T>.Default.Equals(s.TargetValue, targetValue));
            if (match != null) Subscribers.Remove(match.SubscribedFrom);
            _conditionalAsyncSubscribers.RemoveAll(s =>
                s.Callback.Method == subscriber.Method &&
                s.Callback.Target == subscriber.Target &&
                EqualityComparer<T>.Default.Equals(s.TargetValue, targetValue));
        }

        public void ConditionalNoDataUnSubscribeAsync(Func<StateChangeContext, Task> subscriber, T targetValue)
        {
            var match = _conditionalAsyncSubscribersNoData.FirstOrDefault(s =>
                s.Callback.Method == subscriber.Method &&
                s.Callback.Target == subscriber.Target &&
                EqualityComparer<T>.Default.Equals(s.TargetValue, targetValue));
            if (match != null) Subscribers.Remove(match.SubscribedFrom);
            _conditionalAsyncSubscribersNoData.RemoveAll(s =>
                s.Callback.Method == subscriber.Method &&
                s.Callback.Target == subscriber.Target &&
                EqualityComparer<T>.Default.Equals(s.TargetValue, targetValue));
        }

        public void InverseConditionalUnsubscribe(Action<T, StateChangeContext> subscriber, T targetValue)
        {
            var match = _inverseConditionalSubscribers.FirstOrDefault(s =>
                s.Callback.Method == subscriber.Method &&
                s.Callback.Target == subscriber.Target &&
                EqualityComparer<T>.Default.Equals(s.TargetValue, targetValue));
            if (match != null) Subscribers.Remove(match.SubscribedFrom);
            _inverseConditionalSubscribers.RemoveAll(s =>
                s.Callback.Method == subscriber.Method &&
                s.Callback.Target == subscriber.Target &&
                EqualityComparer<T>.Default.Equals(s.TargetValue, targetValue));
        }

        public void InverseConditionalAsyncUnsubscribe(Func<T, StateChangeContext, Task> subscriber, T targetValue)
        {
            var match = _inverseConditionalAsyncSubscribers.FirstOrDefault(s =>
                s.Callback.Method == subscriber.Method &&
                s.Callback.Target == subscriber.Target &&
                EqualityComparer<T>.Default.Equals(s.TargetValue, targetValue));
            if (match != null) Subscribers.Remove(match.SubscribedFrom);
            _inverseConditionalAsyncSubscribers.RemoveAll(s =>
                s.Callback.Method == subscriber.Method &&
                s.Callback.Target == subscriber.Target &&
                EqualityComparer<T>.Default.Equals(s.TargetValue, targetValue));
        }

        public void InverseConditionalAsyncUnsubscribeNoData(Func<StateChangeContext, Task> subscriber, T targetValue)
        {
            var match = _inverseConditionalAsyncSubscribersNoData.FirstOrDefault(s =>
                s.Callback.Method == subscriber.Method &&
                s.Callback.Target == subscriber.Target &&
                EqualityComparer<T>.Default.Equals(s.TargetValue, targetValue));
            if (match != null) Subscribers.Remove(match.SubscribedFrom);
            _inverseConditionalAsyncSubscribersNoData.RemoveAll(s =>
                s.Callback.Method == subscriber.Method &&
                s.Callback.Target == subscriber.Target &&
                EqualityComparer<T>.Default.Equals(s.TargetValue, targetValue));
        }

        // -------------------------
        // Manual conditional checks
        // -------------------------

        public void CheckAndNotifyConditional(T targetValue)
        {
            if (EqualityComparer<T>.Default.Equals(Value, targetValue))
            {
                foreach (var subscriber in _conditionalSubscribers)
                {
                    if (EqualityComparer<T>.Default.Equals(targetValue, subscriber.TargetValue))
                        subscriber.Callback(Value, _context);
                }
                _ = NotifyConditionalAsyncSubscribersForTarget(targetValue);
            }
        }

        public void CheckAndNotifyInverseConditional(T targetValue)
        {
            if (!EqualityComparer<T>.Default.Equals(Value, targetValue))
            {
                foreach (var subscriber in _inverseConditionalSubscribers)
                {
                    if (EqualityComparer<T>.Default.Equals(targetValue, subscriber.TargetValue))
                        subscriber.Callback(Value, _context);
                }
                _ = NotifyInverseConditionalAsyncSubscribersForTarget(targetValue);
            }
        }

        private async Task NotifyConditionalAsyncSubscribersForTarget(T targetValue)
        {
            foreach (var subscriber in _conditionalAsyncSubscribers)
            {
                if (EqualityComparer<T>.Default.Equals(targetValue, subscriber.TargetValue))
                    await subscriber.Callback(Value, _context);
            }

            foreach (var subscriber in _conditionalAsyncSubscribersNoData)
            {
                if (EqualityComparer<T>.Default.Equals(targetValue, subscriber.TargetValue))
                    await subscriber.Callback(_context);
            }
        }

        private async Task NotifyInverseConditionalAsyncSubscribersForTarget(T targetValue)
        {
            foreach (var subscriber in _inverseConditionalAsyncSubscribers)
            {
                if (EqualityComparer<T>.Default.Equals(targetValue, subscriber.TargetValue))
                    await subscriber.Callback(Value, _context);
            }

            foreach (var subscriber in _inverseConditionalAsyncSubscribersNoData)
            {
                if (EqualityComparer<T>.Default.Equals(targetValue, subscriber.TargetValue))
                    await subscriber.Callback(_context);
            }
        }
    }

    // Helper classes for conditional subscribers
    public class ConditionalSubscriber<T>
    {
        public Action<T, StateChangeContext> Callback { get; set; }
        public T TargetValue { get; set; }
        public string SubscribedFrom { get; set; }
    }

    public class ConditionalAsyncSubscriber<T>
    {
        public Func<T, StateChangeContext, Task> Callback { get; set; }
        public T TargetValue { get; set; }
        public string SubscribedFrom { get; set; }
    }

    public class ConditionalAsyncSubscriberNoData<T>
    {
        public Func<StateChangeContext, Task> Callback { get; set; }
        public T TargetValue { get; set; }
        public string SubscribedFrom { get; set; }
    }
}