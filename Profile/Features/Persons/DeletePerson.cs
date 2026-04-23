using FluentValidation;
using Profile.DomainModels.Persons;
using Profile.Grpc;
using Profile.Infrastructure.Database.Store;
using SharedKernel.Domain;
using SharedKernel.Infrastructure.Helper;

namespace Profile.Features.Persons;

public static class DeletePerson
{
    public class Validator : AbstractValidator<DeletePersonGrpcRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0");
        }
    }

    public class DeletePersonHandler(PersonStore personStore) :IHandler
    {
        public async Task<Result<DeletePersonGrpcResponse>> Handle(
            DeletePersonGrpcRequest request,
            IValidator<DeletePersonGrpcRequest> validator,
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<DeletePersonGrpcResponse>(
                    Error.Validation("DeletePerson.ValidationFailed", 
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            return !personStore.Remove(request.Id) ? 
                Result.Failure<DeletePersonGrpcResponse>(PersonErrors.NotFound) : Result.Success(new DeletePersonGrpcResponse { Success = true });

        }
    }
}