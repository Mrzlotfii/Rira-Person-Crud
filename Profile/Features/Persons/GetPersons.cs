using Profile.Grpc;
using Profile.Infrastructure.Database.Store;
using SharedKernel.Domain;
using SharedKernel.Infrastructure.Helper;

namespace Profile.Features.Persons;

public class GetPersons
{
    public class GetPersonsHandler(PersonStore personStore) :IHandler
    {
        public Task<Result<GetPersonsGrpcResponse>> Handle()
        {
            var response = new GetPersonsGrpcResponse();

            foreach (var person in personStore.GetAll())
            {
                response.Persons.Add(new PersonGrpc()
                {
                    Id = person.Id,
                    NationalCode = person.NationalCode,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    DateOfBirth = person.DateOfBirth.ToString("yyyy-MM-dd"),
                    Age = person.Age
                });
            }

            return Task.FromResult(Result.Success(response));
        }
    }
}
