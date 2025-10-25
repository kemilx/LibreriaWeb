namespace SIGEBI.Domain.Base;

public static class DomainValidation
{
    public static string Required(string value, int maxLength, string fieldDisplayName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"El {fieldDisplayName} es obligatorio.", fieldDisplayName);
        }

        var trimmed = value.Trim();

        if (trimmed.Length > maxLength)
        {
            throw new DomainException($"El {fieldDisplayName} no puede exceder {maxLength} caracteres.", fieldDisplayName);
        }

        return trimmed;
    }

    public static string? Optional(string? value, int maxLength, string fieldDisplayName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();

        if (trimmed.Length > maxLength)
        {
            throw new DomainException($"El {fieldDisplayName} no puede exceder {maxLength} caracteres.", fieldDisplayName);
        }

        return trimmed;
    }
}
