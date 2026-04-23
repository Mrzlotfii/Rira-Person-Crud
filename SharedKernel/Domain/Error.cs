namespace SharedKernel.Domain;

public record Error
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new(
        "General.Null",
        "Null value was provided",
        ErrorType.Failure);

    public Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public string Code { get; }

    public string Description { get; }

    public ErrorType Type { get; }

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Problem(string code, string description) =>
        new(code, description, ErrorType.Problem);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);
    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);
    
    public static Error ValidationErrors(IEnumerable<Error> errors)
    {
        var formattedErrors = errors.Select(e => $"{e.Code}: {e.Description}").ToList();
        return new Error("Validation.Error", string.Join(", ", formattedErrors), ErrorType.Validation);
    }
}

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    Problem = 2,
    NotFound = 3,
    Conflict = 4,
    Invalid = 5
}
