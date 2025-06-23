using System;

namespace Wisgenix.DTO.Validators;

public class ValidationHelpers
{
    private static readonly char[] InvalidChars = { '>', '<', '&', '"', '\'' };

    public static bool ContainsInvalidChars(string input)
    {
        return input.Any(c => InvalidChars.Contains(c));
    }
}
