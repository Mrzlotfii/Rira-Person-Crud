using FluentValidation;
using Profile.DomainModels.Persons;
using Profile.Grpc;
using Profile.Infrastructure.Database.Store;
using SharedKernel.Domain;
using SharedKernel.Infrastructure.Helper;

namespace Profile.Features.Persons;

public static class UpdatePerson
{
    public class Validator : AbstractValidator<UpdatePersonGrpcRequest>
    {
        public Validator()
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

    public class UpdatePersonHandler(PersonStore personStore) :IHandler
    {
        public async Task<Result<GrpcPersonResponse>> Handle(
            UpdatePersonGrpcRequest request,
            IValidator<UpdatePersonGrpcRequest> validator,
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<GrpcPersonResponse>(
                    Error.Validation("UpdatePerson.ValidationFailed", 
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            if (!personStore.TryGet(request.Id, out var person))
                return Result.Failure<GrpcPersonResponse>(PersonErrors.NotFound);

            if (!DateTime.TryParseExact(request.DateOfBirth, "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var dateOfBirth))
            {
                return Result.Failure<GrpcPersonResponse>(
                    Error.Validation("UpdatePerson.InvalidDate", "Invalid date format"));
            }

            var updateResult = person?.Update(
                request.FirstName,
                request.LastName,
                dateOfBirth);

            if (updateResult != null && updateResult.IsFailure)
                return Result.Failure<GrpcPersonResponse>(updateResult.Error);

            var response = new GrpcPersonResponse
            {
                Person = new PersonGrpc()
                {
                    Id = person.Id,
                    NationalCode = person.NationalCode,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    DateOfBirth = person.DateOfBirth.ToString("yyyy-MM-dd"),
                    Age = person.Age
                }
            };

            return Result.Success(response);
        }

    }
}