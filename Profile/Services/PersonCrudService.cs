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

