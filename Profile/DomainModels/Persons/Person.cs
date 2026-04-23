using SharedKernel.Domain;

namespace Profile.DomainModels.Persons;

public sealed class Person : BaseEntity<long>
{
    private Person()
    {
    }

    public long PersonId => Id;

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string NationalCode { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    
    public int Age => CalculateAge();

    public static Result<Person> Create(
        long id,
        string nationalCode,
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
            return Result.Failure<Person>(PersonErrors.NationalCodeRequired);

        if (nationalCode.Length != 10 || !nationalCode.All(char.IsDigit))
            return Result.Failure<Person>(PersonErrors.InvalidNationalCode);

        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<Person>(PersonErrors.FirstNameRequired);

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<Person>(PersonErrors.LastNameRequired);

        if (dateOfBirth >= DateTime.UtcNow)
            return Result.Failure<Person>(PersonErrors.InvalidDateOfBirth);

        var person = new Person
        {
            Id = id,
            NationalCode = nationalCode,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            CreatedAtUtc = createdAtUtc,
            ModifiedAtUtc = createdAtUtc
        };

        return person;
    }

    public Result Update(string firstName, string lastName, DateTime dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure(PersonErrors.FirstNameRequired);

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure(PersonErrors.LastNameRequired);

        if (dateOfBirth >= DateTime.UtcNow)
            return Result.Failure(PersonErrors.InvalidDateOfBirth);

        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        ModifiedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    private int CalculateAge()
    {
        var today = DateTime.UtcNow;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
}

