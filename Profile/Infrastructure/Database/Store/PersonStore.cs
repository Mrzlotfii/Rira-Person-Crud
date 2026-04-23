using Profile.DomainModels.Persons;

namespace Profile.Infrastructure.Database.Store;

public class PersonStore
{
    private readonly Dictionary<long, Person> _personsById = new();
    private readonly Dictionary<string, long> _nationalCodeIndex = new();
    private readonly Lock _lock = new();

    public void Add(Person person)
    {
        lock (_lock)
        {
            _personsById[person.Id] = person;
        }
    }

    public bool ExistsByNationalCode(string nationalCode)
    {
        lock (_lock)
        {
            return _nationalCodeIndex.ContainsKey(nationalCode);
        }
    }

    public bool Remove(long id)
    {
        lock (_lock)
        {
            if (!_personsById.Remove(id, out var person))
                return false;

            _nationalCodeIndex.Remove(person.NationalCode);
            return true;
        }
    }

    public bool TryGet(long id, out Person? person)
    {
        lock (_lock)
        {
            return _personsById.TryGetValue(id, out person);
        }
    }

    public IEnumerable<Person> GetAll()
    {
        lock (_lock)
        {
            return _personsById.Values.ToList();
        }
    }
}
