using FluentValidation;
using Profile.DomainModels.Persons;
using Profile.Grpc;
using Profile.Infrastructure.Database.Store;
using SharedKernel.Domain;
using SharedKernel.Infrastructure.Helper;

namespace Profile.Features.Persons;

public static class GetPerson
{
    public class Validator : AbstractValidator<GetPersonGrpcRequest>
    {
        public Validator()
        {
        }
    }

    public class GetPersonHandler(PersonStore personStore) :IHandler
    {
        public async Task<Result<GrpcPersonResponse>> Handle(
            GetPersonGrpcRequest request,
            IValidator<GetPersonGrpcRequest> validator,
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<GrpcPersonResponse>(
                    Error.Validation("GetPerson.ValidationFailed", 
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            if (!personStore.TryGet(request.Id, out var person))
                return Result.Failure<GrpcPersonResponse>(PersonErrors.NotFound);

            var response = new GrpcPersonResponse
            {
                Person = new  PersonGrpc()
                {
                    Id = person!.Id,
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