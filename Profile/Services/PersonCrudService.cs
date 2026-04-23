using FluentValidation;
using Grpc.Core;
using Profile.Features.Persons;
using Profile.Grpc;
using SharedKernel.Domain;

namespace Profile.Services;

public class PersonCrudService(
    IValidator<CreatePersonGrpcRequest> createPersonValidator,
    IValidator<UpdatePersonGrpcRequest> updatePersonValidator,
    IValidator<DeletePersonGrpcRequest> deletePersonValidator,
    IValidator<GetPersonGrpcRequest> getPersonValidator,
    CreatePerson.CreatePersonHandler createPersonHandler,
    UpdatePerson.UpdatePersonHandler updatePersonHandler,
    DeletePerson.DeletePersonHandler deletePersonHandler,
    GetPerson.GetPersonHandler getPersonHandler,
    GetPersons.GetPersonsHandler listPersonsHandler) : PersonCrud.PersonCrudBase
{
    public override async Task<GrpcPersonResponse> CreatePerson(
        CreatePersonGrpcRequest request,
        ServerCallContext context)
    {
        var result = await createPersonHandler.Handle(
            request,
            createPersonValidator,
            context.CancellationToken);

        if (result.IsFailure)
        {
            throw result.Error.Type switch
            {
                ErrorType.Validation => new RpcException(new Status(StatusCode.InvalidArgument, result.Error.Description)),
                ErrorType.Conflict => new RpcException(new Status(StatusCode.AlreadyExists, result.Error.Description)),
                _ => new RpcException(new Status(StatusCode.Internal, result.Error.Description))
            };
        }

        return result.Value;
    }

    public override async Task<GrpcPersonResponse> GetPerson(
        GetPersonGrpcRequest request,
        ServerCallContext context)
    {
        var result = await getPersonHandler.Handle(
            request,
            getPersonValidator,
            context.CancellationToken);
    
        if (result.IsFailure)
        {
            throw result.Error.Type switch
            {
                ErrorType.NotFound => new RpcException(new Status(StatusCode.NotFound, result.Error.Description)),
                ErrorType.Validation => new RpcException(new Status(StatusCode.InvalidArgument, result.Error.Description)),
                _ => new RpcException(new Status(StatusCode.Internal, result.Error.Description))
            };
        }

        return result.Value;
    }

    public override async Task<GrpcPersonResponse> UpdatePerson(
        UpdatePersonGrpcRequest request,
        ServerCallContext context)
    {
        var result = await updatePersonHandler.Handle(
            request,
            updatePersonValidator,
            context.CancellationToken);
        
        if (result.IsFailure)
        {
            throw result.Error.Type switch
            {
                ErrorType.NotFound => new RpcException(new Status(StatusCode.NotFound, result.Error.Description)),
                ErrorType.Validation => new RpcException(new Status(StatusCode.InvalidArgument, result.Error.Description)),
                _ => new RpcException(new Status(StatusCode.Internal, result.Error.Description))
            };
        }

        return result.Value;
    }

    public override async Task<DeletePersonGrpcResponse> DeletePerson(
        DeletePersonGrpcRequest request,
        ServerCallContext context)
    {
        var result = await deletePersonHandler.Handle(
            request,
            deletePersonValidator,
            context.CancellationToken);

        if (result.IsFailure)
        {
            throw result.Error.Type switch
            {
                ErrorType.Validation => new RpcException(new Status(StatusCode.InvalidArgument, result.Error.Description)),
                ErrorType.NotFound => new RpcException(new Status(StatusCode.NotFound, result.Error.Description)),
                _ => new RpcException(new Status(StatusCode.Internal, result.Error.Description))
            };
        }

        return result.Value;
    }


    public override async Task<GetPersonsGrpcResponse> GetPersons(
        GetPersonsGrpcRequest request,
        ServerCallContext context)
    {
        var result = await listPersonsHandler.Handle();

        return result.IsFailure ? 
            throw new RpcException(new Status(StatusCode.Internal, result.Error.Description)) : result.Value;

    }
}


// using Grpc.Core;
// using FluentValidation;
//
// namespace PersonService.Services;
//
// public class PersonCrudService(
//     ILogger<PersonCrudService> logger,
//     IValidator<CreatePersonRequest> createValidator,
//     IValidator<UpdatePersonRequest> updateValidator) : PersonCrud.PersonCrudBase
// {
//     private static readonly Dictionary<string, Person> _store = new();
//
//     public override async Task<PersonResponse> CreatePerson(CreatePersonRequest request, ServerCallContext context)
//     {
//         var validationResult = await createValidator.ValidateAsync(request);
//         if (!validationResult.IsValid)
//         {
//             var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
//             throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
//         }
//
//         if (_store.ContainsKey(request.NationalCode))
//             throw new RpcException(new Status(StatusCode.AlreadyExists, $"Person with national code {request.NationalCode} already exists"));
//
//         var person = new Person
//         {
//             FirstName = request.FirstName,
//             LastName = request.LastName,
//             NationalCode = request.NationalCode,
//             DateOfBirth = request.DateOfBirth
//         };
//
//         _store[person.NationalCode] = person;
//         logger.LogInformation("Created person {NationalCode}", person.NationalCode);
//         
//         return new PersonResponse { Person = person };
//     }
//
//     public override Task<PersonResponse> GetPerson(GetPersonRequest request, ServerCallContext context)
//     {
//         if (!_store.TryGetValue(request.NationalCode, out var person))
//             throw new RpcException(new Status(StatusCode.NotFound, $"Person with national code {request.NationalCode} not found"));
//
//         return Task.FromResult(new PersonResponse { Person = person });
//     }
//
//     public override async Task<PersonResponse> UpdatePerson(UpdatePersonRequest request, ServerCallContext context)
//     {
//         if (!_store.ContainsKey(request.NationalCode))
//             throw new RpcException(new Status(StatusCode.NotFound, $"Person with national code {request.NationalCode} not found"));
//
//         var validationResult = await updateValidator.ValidateAsync(request);
//         if (!validationResult.IsValid)
//         {
//             var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
//             throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
//         }
//
//         var updated = new Person
//         {
//             FirstName = request.FirstName,
//             LastName = request.LastName,
//             NationalCode = request.NationalCode,
//             DateOfBirth = request.DateOfBirth
//         };
//
//         _store[request.NationalCode] = updated;
//         logger.LogInformation("Updated person {NationalCode}", updated.NationalCode);
//         
//         return new PersonResponse { Person = updated };
//     }
//
//     public override Task<DeletePersonResponse> DeletePerson(DeletePersonRequest request, ServerCallContext context)
//     {
//         bool removed = _store.Remove(request.NationalCode);
//         
//         if (!removed)
//             throw new RpcException(new Status(StatusCode.NotFound, $"Person with national code {request.NationalCode} not found"));
//
//         logger.LogInformation("Deleted person {NationalCode}", request.NationalCode);
//         return Task.FromResult(new DeletePersonResponse { Success = true });
//     }
//
//     public override Task<ListPersonsResponse> ListPersons(ListPersonsRequest request, ServerCallContext context)
//     {
//         var response = new ListPersonsResponse();
//         response.Persons.AddRange(_store.Values.OrderBy(p => p.NationalCode));
//         return Task.FromResult(response);
//     }
// }
