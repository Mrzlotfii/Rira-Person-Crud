using SharedKernel.Domain;

namespace Profile.DomainModels.Persons;

public static class PersonErrors
{
    public static readonly Error NationalCodeRequired = 
        Error.Validation("Person.NationalCodeRequired", "National code is required");
    
    public static readonly Error InvalidNationalCode = 
        Error.Validation("Person.InvalidNationalCode", "National code must be 10 digits");
    
    public static readonly Error FirstNameRequired = 
        Error.Validation("Person.FirstNameRequired", "First name is required");
    
    public static readonly Error LastNameRequired = 
        Error.Validation("Person.LastNameRequired", "Last name is required");
    
    public static readonly Error InvalidDateOfBirth = 
        Error.Validation("Person.InvalidDateOfBirth", "Date of birth must be in the past");
    
    public static readonly Error NotFound = 
        Error.NotFound("Person.NotFound", "Person not found");
    
    public static readonly Error AlreadyExists = 
        Error.Conflict("Person.AlreadyExists", "Person with this national code already exists");
}

