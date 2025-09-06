using System.Net;
using System.Net.Http.Json;
using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace api.IntegrationTests;

public class TodoControllerTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16.1")
        .WithDatabase("test_db")
        .WithUsername("test_user")
        .WithPassword("test_password")
        .Build();

    private TodoApiFactory _factory;
    private HttpClient _client;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _factory = new TodoApiFactory(_dbContainer.GetConnectionString());
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
    }
    
    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await _factory.DisposeAsync();
    }
    
    private async Task SeedDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

        dbContext.Todos.Add(new Todo()
        {
            Id = new Guid("fe84672e-302e-460e-b292-938ac417ccda"),
            Name = "Walk the dog",
            Description = "Take the dog for a walk",
            Completed = false,
            Position = 0,
            CreatedAt = new DateTimeOffset(2025, 09, 06, 10, 55, 12, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2025, 09, 06, 10, 55, 12, TimeSpan.Zero),
        });

        dbContext.Todos.Add(new Todo()
        {
            Id = new Guid("a72d3a45-c580-4152-bf3b-67b33b18a3aa"),
            Name = "Clean the house",
            Description = "Deep clean the house including the bathroom",
            Completed = true,
            Position = 1,
            CreatedAt = new DateTimeOffset(2025, 08, 04, 06, 22, 42, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2025, 08, 04, 06, 22, 42, TimeSpan.Zero),
        });

        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllTodos_ReturnsSuccessAndCorrectContentType()
    {
        await SeedDatabaseAsync();

        var response = await _client.GetAsync("/api/Todo");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task GetAllTodos_ReturnsExpectedResponseBody()
    {
        await SeedDatabaseAsync();
        
        var responseBody = await _client.GetFromJsonAsync<TodosResponse>("/api/todo");

        Assert.NotNull(responseBody);
        Assert.Equal(2, responseBody.Items.Count());
        Assert.Equal(2, responseBody.TotalCount);
    }

    [Fact]
    public async Task GetTodoReturns_ExistingTodo()
    {
        await SeedDatabaseAsync();
        
        const string existingTodoId = "fe84672e-302e-460e-b292-938ac417ccda";
        var response = await _client.GetAsync($"/api/todo/{existingTodoId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var returnedTodo = await response.Content.ReadFromJsonAsync<TodoResponse>();

        Assert.NotNull(returnedTodo);
        
        Assert.Equal("Walk the dog", returnedTodo.Name);
        Assert.Equal("Take the dog for a walk", returnedTodo.Description);
        Assert.False(returnedTodo.Completed);
        Assert.Equal(0, returnedTodo.Position);
        Assert.Equal(
            new DateTimeOffset(2025, 09, 06, 10, 55, 12, TimeSpan.Zero), 
            returnedTodo.CreatedAt);
        Assert.Equal(
            new DateTimeOffset(2025, 09, 06, 10, 55, 12, TimeSpan.Zero),
            returnedTodo.UpdatedAt);
    }
    

    [Fact]
    public async Task GetTodoReturnsNotFound_WhenRandomId()
    {
        var response = await _client.GetAsync($"api/Todo/{Guid.NewGuid()}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_Returns_CreatedAndLocationHeader_ForValidRequest()
    {
        var requestContent = new CreateTodoRequest()
        {
            Name = "Walk the dog",
            Description = "Take the dog for a walk."
        };

        var response = await _client.PostAsJsonAsync("/api/todo", requestContent);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var getResponse = await _client.GetAsync(response.Headers.Location);

        getResponse.EnsureSuccessStatusCode();

        var createdTodo = await getResponse.Content.ReadFromJsonAsync<TodoResponse>();

        Assert.NotNull(createdTodo);
        Assert.Equal(requestContent.Name, createdTodo.Name);
        Assert.Equal(requestContent.Description, createdTodo.Description);
    }
    
    
    [Fact]
    public async Task CreateTodo_Returns_BadRequest_WhenInvalidRequest()
    {
        var requestContent = new CreateTodoRequest()
        {
        };

        var response = await _client.PostAsJsonAsync("/api/Todo", requestContent);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}