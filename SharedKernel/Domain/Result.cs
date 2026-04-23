using System.Diagnostics.CodeAnalysis;

namespace SharedKernel.Domain;

public class Result
{
    protected Result(bool isSuccess, Error error, IEnumerable<Error>? errors)
    {
        IsSuccess = isSuccess;
        Error = error;

        var errorList = errors?.ToList() ?? (error != Error.None ? [error] : []);
        if (isSuccess && errorList.Any())
        {
            throw new ArgumentException("Successful result must not contain errors.", nameof(error));
        }

        if (!isSuccess && !errorList.Any())
        {
            throw new ArgumentException("Failed result must contain at least one error.", nameof(errors));
        }

        Errors = errorList;
    }

    protected Result(bool isSuccess, Error error)
        : this(isSuccess, error, null)
    {
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public Error Error { get; }
    public IReadOnlyList<Error> Errors { get; } = [];

    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, Error.None);

    public static Result Failure(Error error) =>
        new(false, error, [error]);

    public static Result Failure(IEnumerable<Error> errors)
    {
        var list = errors?.ToList() ?? throw new ArgumentNullException(nameof(errors));
        return new Result(false, list.FirstOrDefault() ?? Error.None, list);
    }
    
    public static Result<TValue> Failure<TValue>(Error error) =>
        new(default, false, error, [error]);

    public static Result<TValue> Failure<TValue>(IEnumerable<Error> errors)
    {
        var list = errors?.ToList() ?? throw new ArgumentNullException(nameof(errors));
        return new(default, false, list.FirstOrDefault() ?? Error.None, list);
    }
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error, IEnumerable<Error>? errors = null)
        : base(isSuccess, error, errors)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot access the value of a failed result.");

    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure(Error.NullValue);

    public static Result<TValue> Success(TValue value) =>
        new(value, true, Error.None);

    public static new Result<TValue> Failure(Error error) =>
        new(default, false, error, [error]);

    public static new Result<TValue> Failure(IEnumerable<Error> errors)
    {
        var list = errors?.ToList() ?? throw new ArgumentNullException(nameof(errors));
        return new Result<TValue>(default, false, list.FirstOrDefault() ?? Error.None, list);
    }

    public static Result<TValue> ValidationFailure(Error error) =>
        new(default, false, error);
}
