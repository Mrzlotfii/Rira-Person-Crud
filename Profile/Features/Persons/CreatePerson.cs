using FluentValidation;
using Profile.DomainModels.Persons;
using Profile.Grpc;
using Profile.Infrastructure.Database.Store;
using SharedKernel.Domain;
using SharedKernel.Infrastructure.Helper;

namespace Profile.Features.Persons;

public static class CreatePerson
{
    public class CreatePersonRequestValidator : AbstractValidator<CreatePersonGrpcRequest>
    {
        public CreatePersonRequestValidator()
        {
            RuleFor(x => x.NationalCode)
                .NotEmpty()
                .Length(10)
                .Matches("^[0-9]+$");

            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .Must(BeValidDate).WithMessage("Invalid date format. Use YYYY-MM-DD");
        }

        private bool BeValidDate(string dateString)
        {
            return DateTime.TryParseExact(dateString, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out _);
        }
    }

    public class CreatePersonHandler(PersonStore store,ISnowflakeGenerator snowflakeGenerator) : IHandler
    {
        public async Task<Result<GrpcPersonResponse>> Handle(
            CreatePersonGrpcRequest request,
            IValidator<CreatePersonGrpcRequest> validator,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<GrpcPersonResponse>(
                    Error.Validation("CreatePerson.ValidationFailed",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            if (store.ExistsByNationalCode(request.NationalCode))
                return Result.Failure<GrpcPersonResponse>(PersonErrors.AlreadyExists);

            if (!DateTime.TryParseExact(request.DateOfBirth, "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var dateOfBirth))
            {
                return Result.Failure<GrpcPersonResponse>(
                    Error.Validation("CreatePerson.InvalidDate", "Invalid date format. Expected yyyy-MM-dd"));
            }
            var personId = snowflakeGenerator.Generate();

            var personResult = Person.Create(
                personId,
                request.NationalCode,
                request.FirstName,
                request.LastName,
                dateOfBirth,
                DateTime.UtcNow);

            if (personResult.IsFailure)
                return Result.Failure<GrpcPersonResponse>(personResult.Error);

            store.Add(personResult.Value);

            var response = new GrpcPersonResponse
            {
                Person = new PersonGrpc()
                {
                    Id = personResult.Value.Id,
                    NationalCode = personResult.Value.NationalCode,
                    FirstName = personResult.Value.FirstName,
                    LastName = personResult.Value.LastName,
                    DateOfBirth = personResult.Value.DateOfBirth.ToString("yyyy-MM-dd"),
                    Age = personResult.Value.Age
                }
            };

            return Result.Success(response);
        }
    }
}
