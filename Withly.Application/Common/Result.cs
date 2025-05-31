namespace Withly.Application.Common;

public class Result<T>
{
    public bool IsSuccess => Error == null;
    public string? Error { get; }
    public T? Value { get; }

    private Result(T value)
    {
        Value = value;
    }

    private Result(string error)
    {
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Fail(string error) => new(error);
}