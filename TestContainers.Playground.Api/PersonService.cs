using Microsoft.EntityFrameworkCore;
using TestContainers.Playground.Core;

namespace TestContainers.Playground.Api;

public class PersonService
{
    private readonly AppDbContext dbContext;

    public PersonService(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    
    public async Task<Core.Entities.Person> CreatePersonAsync(string firstName, string lastName)
    {
        var person = new Core.Entities.Person
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName
        };
        
        await dbContext.Persons.AddAsync(person);
        await dbContext.SaveChangesAsync();
        return person;
    }

    public async Task<IEnumerable<Core.Entities.Person>> GetPersonsAsync()
    {
        return await dbContext.Persons.ToListAsync();
    }
}