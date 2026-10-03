using System.Diagnostics.CodeAnalysis;

namespace Supermercado.Api.Services;

public enum ErrorType
{
    NotFound,
    Conflict,
    Validation,
}

public sealed record Error(ErrorType Type, string Message);

public sealed class Result<T>
{
    private Result(T? value, Error? error)
    {
        Value = value;
        Error = error;
    }

    public T? Value { get; }
    public Error? Error { get; }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error is null;

    public static implicit operator Result<T>(T value) => new(value, null);
    public static implicit operator Result<T>(Error error) => new(default, error);
}
