using System.Net.Http.Json;
using TestContainers.Playground.Core;
using TestContainers.Playground.Core.Entities;

namespace TestContainers.PlayGround.Tests;

public class PersonServiceTests(IntegrationTestWebApplicationFactory<Program, AppDbContext> factory) : AspnetCoreIntegrationFixture(factory)
{
    [Fact]
    public async Task CreatePersonAsync_ShouldCreatePerson()
    {
        var client = Factory.CreateClient();

        var postResponse = await client.PostAsJsonAsync("/persons", new { firstName = "Donald", lastName = "Duck" });
        postResponse.EnsureSuccessStatusCode();

        var fetchResponse = await client.GetAsync("/persons");
        fetchResponse.EnsureSuccessStatusCode();

        var persons = (await fetchResponse.Content.ReadFromJsonAsync<IList<Person>>());
        Assert.NotNull(persons);
        Assert.Single(persons);
        Assert.Equal("Donald", persons.First().FirstName);
        Assert.Equal("Duck", persons.First().LastName);
    }

    [Fact]
    public async Task CreatePersonAsync_ShouldCreateMultiplePersons()
    {
        var client = Factory.CreateClient();

        var postResponse = await client.PostAsJsonAsync("/persons", new { firstName = "Donald", lastName = "Duck" });
        postResponse.EnsureSuccessStatusCode();

        postResponse = await client.PostAsJsonAsync("/persons", new { firstName = "Popeye", lastName = "The Sailorman" });
        postResponse.EnsureSuccessStatusCode();

        var fetchResponse = await client.GetAsync("/persons");
        fetchResponse.EnsureSuccessStatusCode();

        var persons = (await fetchResponse.Content.ReadFromJsonAsync<IList<Person>>());
        Assert.NotNull(persons);
        Assert.Equal(2, persons.Count);
    }
}