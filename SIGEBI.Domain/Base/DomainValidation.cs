using System;

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

    public static int Positive(int value, string fieldDisplayName)
    {
        if (value <= 0)
        {
            throw new DomainException($"El {fieldDisplayName} debe ser mayor que cero.", fieldDisplayName);
        }

        return value;
    }

    public static decimal NonNegative(decimal value, string fieldDisplayName)
    {
        if (value < 0)
        {
            throw new DomainException($"El {fieldDisplayName} no puede ser negativo.", fieldDisplayName);
        }

        return value;
    }

    public static void EnsureDateOrder(DateTime startUtc, DateTime endUtc, string startFieldDisplayName, string endFieldDisplayName)
    {
        if (endUtc <= startUtc)
        {
            throw new DomainException($"La {endFieldDisplayName} debe ser posterior a la {startFieldDisplayName}.", endFieldDisplayName);
        }
    }
}
