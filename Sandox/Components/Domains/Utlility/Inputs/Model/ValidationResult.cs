namespace FE.Components.Shared.Validation;

public record ValidationResult(bool IsValid, string? Label = null);