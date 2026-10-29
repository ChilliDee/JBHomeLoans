using FE.Components.Shared.Validation;

public abstract class ValidatableInputs
{
    // Same tuple shape the input components pass to ValidationRef / ValidationUnref
    private readonly List<Func<(bool IsValid, string Label)>> _validators = new();

    // Func<ValidationResult> validators are adapted to the tuple shape; keep the adapter so the same delegate can be removed
    private readonly Dictionary<Func<ValidationResult>, Func<(bool IsValid, string Label)>> _adapted = new();

    public bool IsValid { get; private set; } = true;
    public List<string> FailedLabels { get; private set; } = new();

    public void AddValidator(Func<(bool IsValid, string Label)> validator) => _validators.Add(validator);

    public void RemoveValidator(Func<(bool IsValid, string Label)> validator) => _validators.Remove(validator);

    public void AddValidator(Func<ValidationResult> validator)
    {
        Func<(bool IsValid, string Label)> adapter = () =>
        {
            var result = validator();
            return (result.IsValid, result.Label ?? string.Empty);
        };
        _adapted[validator] = adapter;
        _validators.Add(adapter);
    }

    public void RemoveValidator(Func<ValidationResult> validator)
    {
        if (_adapted.Remove(validator, out var adapter))
        {
            _validators.Remove(adapter);
        }
    }

    public (bool IsValid, List<string> FailedLabels) ValidateAll()
    {
        // ToList() so a validator that adds/removes validators while running can't break the loop
        var results = _validators.ToList().Select(v => v()).ToList();

        IsValid = results.All(r => r.IsValid);
        FailedLabels = results
            .Where(r => !r.IsValid && !string.IsNullOrWhiteSpace(r.Label))
            .Select(r => r.Label)
            .ToList();

        return (IsValid, FailedLabels);
    }
}
