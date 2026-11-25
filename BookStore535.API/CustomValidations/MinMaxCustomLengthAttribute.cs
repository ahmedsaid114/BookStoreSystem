using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.CustomValidations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class MinMaxCustomLengthAttribute(int minValue, int maxValue) : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string text || string.IsNullOrWhiteSpace(text)) return false;
        var length = text.Trim().Length;
        return length >= minValue && length <= maxValue;
    }

    public override string FormatErrorMessage(string name) =>
        $"The length of {name} must be between {minValue} and {maxValue} characters.";
}

