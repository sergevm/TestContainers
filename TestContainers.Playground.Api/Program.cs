using Microsoft.AspNetCore.Mvc;
using TestContainers.Playground.Api;
using TestContainers.Playground.Api.Contracts;
using TestContainers.Playground.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSqlServer<AppDbContext>(builder.Configuration.GetConnectionString("Docker-Sql"));
    
builder.Services.AddScoped<PersonService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/persons", async (PersonService personService, [FromBody]CreatePersonCommand command) =>
    await personService.CreatePersonAsync(command.FirstName, command.LastName))
    .WithName("CreatePerson")
    .WithOpenApi();

app.MapGet("/persons", async (PersonService personService) => await personService.GetPersonsAsync())
    .WithName("GetPersons")
    .WithOpenApi();

app.Run();

public partial class Program { }