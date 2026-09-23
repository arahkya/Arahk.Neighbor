namespace Arahk.Neighbor.Application.Common;

public class Result
{
    public bool Succeeded { get; init; }
    public string? ErrorKey { get; init; }
    public Dictionary<string, string> FieldErrors { get; init; } = new();
    public int? RemainingAttempts { get; init; }
    public int? CooldownSeconds { get; init; }

    public static Result Ok() => new() { Succeeded = true };

    public static Result Fail(string errorKey, int? remainingAttempts = null, int? cooldownSeconds = null) =>
        new()
        {
            Succeeded = false,
            ErrorKey = errorKey,
            RemainingAttempts = remainingAttempts,
            CooldownSeconds = cooldownSeconds
        };

    public static Result FailFields(Dictionary<string, string> fields) =>
        new() { Succeeded = false, FieldErrors = fields };
}

public class Result<T> : Result
{
    public T? Data { get; init; }

    public static Result<T> Ok(T data) => new() { Succeeded = true, Data = data };

    public new static Result<T> Fail(string errorKey, int? remainingAttempts = null, int? cooldownSeconds = null) =>
        new()
        {
            Succeeded = false,
            ErrorKey = errorKey,
            RemainingAttempts = remainingAttempts,
            CooldownSeconds = cooldownSeconds
        };

    public new static Result<T> FailFields(Dictionary<string, string> fields) =>
        new() { Succeeded = false, FieldErrors = fields };
}
