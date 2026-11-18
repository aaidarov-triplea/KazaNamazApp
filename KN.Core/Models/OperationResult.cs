namespace KN.Core.Models;

public record OperationResult (bool Ok, string? Error = null)
{
    public static OperationResult Success() => new(true);
    public static OperationResult Fail(string error) => new(false, error);
}