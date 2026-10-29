namespace FE.Components.Domains.Utlility.Inputs.Model
{
    public class TrackedInput<T>
    {
        private T _value;
        private T _initialValue;

        /// <summary>
        /// The current value. On first assignment, also sets InitialValue.
        /// On subsequent assignments, pushes the current value into History
        /// before replacing it with the new value.
        /// </summary>
        public T TrackedValue
        {
            get => _value;
            set
            {
                if (!_hasBeenInitialised)
                {
                    _value = value;
                    _initialValue = value;
                    _hasBeenInitialised = true;
                }
                else
                {
                    History.Add(_value);
                    _value = value;
                }
            }
        }

        private bool _hasBeenInitialised = false;

        /// <summary>
        /// The value first assigned to this input. Never changes unless Reset() is called.
        /// </summary>
        public T InitialValue => _initialValue;

        /// <summary>
        /// Every previous value this input has held, in chronological order.
        /// Does not include the current or initial value.
        /// </summary>
        public List<T> History { get; private set; } = new();

        /// <summary>
        /// True if the current value differs from the initial value.
        /// Returns false if the value was changed and then changed back.
        /// </summary>
        public bool HasChanged =>
            _hasBeenInitialised &&
            !EqualityComparer<T>.Default.Equals(_value, _initialValue);

        /// <summary>
        /// The value held before the most recent change, or default if no prior value exists.
        /// </summary>
        public T? PreviousValue =>
            History.Count >= 1 ? History[^1] : default;

        /// <summary>
        /// True if the initial value was non-null / non-default at load time.
        /// </summary>
        public bool HasInitialValue =>
            _hasBeenInitialised &&
            !EqualityComparer<T>.Default.Equals(_initialValue, default);

        public TrackedInput() { }

        public TrackedInput(T initialValue)
        {
            TrackedValue = initialValue;
        }

        /// <summary>
        /// Sets the current value as the new initial value and clears history.
        /// HasChanged will return false until the value changes again.
        /// </summary>
        public void Reset()
        {
            _initialValue = _value;
            History.Clear();
        }    
    }
}