using FE.Components.Shared.Validation;

public abstract class ValidatableInputs
{
    private readonly List<Func<(bool IsValid, string Label)>> _validators = new();
    private readonly Dictionary<Func<ValidationResult>, Func<(bool IsValid, string Label)>> _adapted = new();

    public bool IsValid { get; private set; }
    public List<string> FailedLabels { get; private set; } = new();

    // Tuple shape: the one input components pass to ValidationRef / ValidationUnref
    public void AddValidator(Func<(bool IsValid, string Label)> validator) => _validators.Add(validator);

    public void RemoveValidator(Func<(bool IsValid, string Label)> validator) => _validators.Remove(validator);

    // ValidationResult shape: adapted to the tuple, removable by the same delegate
    public void AddValidator(Func<ValidationResult> validator)
    {
        Func<(bool IsValid, string Label)> adapted = () =>
        {
            var result = validator();
            return (result.IsValid, result.Label ?? string.Empty);
        };
        _adapted[validator] = adapted;
        _validators.Add(adapted);
    }

    public void RemoveValidator(Func<ValidationResult> validator)
    {
        if (_adapted.Remove(validator, out var adapted))
        {
            _validators.Remove(adapted);
        }
    }

    public (bool IsValid, List<string> FailedLabels) ValidateAll()
    {
        var results = _validators.Select(v => v()).ToList();
        IsValid = results.All(r => r.IsValid);
        FailedLabels = results
            .Where(r => !r.IsValid && !string.IsNullOrEmpty(r.Label))
            .Select(r => r.Label)
            .ToList();
        return (IsValid, FailedLabels);
    }
}
