using Microsoft.EntityFrameworkCore;
using TestContainers.Playground.Core.Entities;

namespace TestContainers.Playground.Core;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Person> Persons { get; set; }
}