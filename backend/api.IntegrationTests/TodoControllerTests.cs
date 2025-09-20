using System.Net;
using System.Net.Http.Json;
using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace api.IntegrationTests;

public class TodoControllerTests : IAsyncLifetime
{
    private const string TodosBaseUrl = "/api/v1/todo";
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
            UpdatedAt = new DateTimeOffset(2025, 09, 07, 12, 12, 38, TimeSpan.Zero),
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

        var response = await _client.GetAsync(TodosBaseUrl);

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", response.Content.Headers.ContentType?.CharSet);
    }

    [Fact]
    public async Task GetAllTodos_ReturnsEmptyListWhenDbEmpty()
    {
        var responseBody = await _client.GetFromJsonAsync<TodosResponse>(TodosBaseUrl);

        Assert.NotNull(responseBody);
        Assert.Equal([], responseBody.Items);
        Assert.Equal(0, responseBody.TotalCount);
    }

    [Fact]
    public async Task GetAllTodos_ReturnsExpectedResponseBody()
    {
        await SeedDatabaseAsync();
        
        var responseBody = await _client.GetFromJsonAsync<TodosResponse>(TodosBaseUrl);

        Assert.NotNull(responseBody);
        Assert.Equal(2, responseBody.Items.Count());
        Assert.Equal(2, responseBody.TotalCount);

        var items = responseBody.Items.ToList();
        Assert.Contains(items, t => t is { Name: "Walk the dog", Completed: false });
        Assert.Contains(items, t => t is { Name: "Clean the house", Completed: true });
    }

    [Fact]
    public async Task GetTodoReturns_ExistingTodo()
    {
        await SeedDatabaseAsync();
        
        const string existingTodoId = "fe84672e-302e-460e-b292-938ac417ccda";
        var response = await _client.GetAsync($"{TodosBaseUrl}/{existingTodoId}");
        
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
            new DateTimeOffset(2025, 09, 07, 12, 12, 38, TimeSpan.Zero),
            returnedTodo.UpdatedAt);
    }
    
    [Fact]
    public async Task GetTodoReturnsNotFound_WhenRandomId()
    {
        var response = await _client.GetAsync($"{TodosBaseUrl}/{Guid.NewGuid()}");
        
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

        var response = await _client.PostAsJsonAsync(TodosBaseUrl, requestContent);
        
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

        var response = await _client.PostAsJsonAsync(TodosBaseUrl, requestContent);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(error);
        Assert.Contains("Name", error.Errors.Keys);
    }

    [Fact]
    public async Task CreateTodo_Returns_BadRequest_WhenTooShortDescription()
    {
        var requestContent = new CreateTodoRequest()
        {
            Name = "Test todo",
            Description = "ab"
        };

        var response = await _client.PostAsJsonAsync(TodosBaseUrl, requestContent);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(error);
        Assert.Contains("Description", error.Errors.Keys);
    }

    [Fact]
    public async Task UpdateTodo_Successfully_UpdatesTodo_WhenValid()
    {
        await SeedDatabaseAsync();
        
        var updateContent = new UpdateTodoRequest()
        {
            Name = "Updated name",
            Description = "Updated description",
            Completed = true,
        };

        var response = await _client.PatchAsJsonAsync(
            $"{TodosBaseUrl}/fe84672e-302e-460e-b292-938ac417ccda", 
            updateContent);
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponseBody =
            await _client.GetFromJsonAsync<TodoResponse>($"{TodosBaseUrl}/fe84672e-302e-460e-b292-938ac417ccda");

        Assert.NotNull(getResponseBody);
        Assert.Equal("Updated name", getResponseBody.Name);
        Assert.Equal("Updated description", getResponseBody.Description);
        Assert.True(getResponseBody.Completed);
    }

    [Fact]
    public async Task UpdateTodo_ReturnsNotFound_WhenTodoDoesNotExist()
    {
        var randomId = Guid.NewGuid();
        var updatedTodo = new UpdateTodoRequest()
        {
            Name = "Updated todo",
            Description = "Test description",
            Completed = true,
        };
        
        var response = await _client.PatchAsJsonAsync($"{TodosBaseUrl}/{randomId}", updatedTodo);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_ReturnsBadRequest_WhenInvalidRequestContent()
    {
        await SeedDatabaseAsync();
        
        var updatedTodo = new UpdateTodoRequest()
        {
            Name = "aa",
        }; 
        
        var response = await _client.PatchAsJsonAsync($"{TodosBaseUrl}/fe84672e-302e-460e-b292-938ac417ccda", updatedTodo);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_Returns_NotFound_ForNonExistentTodo()
    {
        var randomId = Guid.NewGuid();
        var response = await _client.DeleteAsync($"{TodosBaseUrl}/{randomId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_Deletes_ExistingTodo()
    {
        await SeedDatabaseAsync();
        
        var deleteResponse = await _client.DeleteAsync($"{TodosBaseUrl}/fe84672e-302e-460e-b292-938ac417ccda");
        
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"{TodosBaseUrl}/fe84672e-302e-460e-b292-938ac417ccda");
        
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task ReorderTodos_ReturnsNotContentStatus_WhenValid()
    {
        await SeedDatabaseAsync();
        
        var requestBody = new ReorderTodosRequest()
        {
            Todos = new List<TodoPosition>()
            {
                new()
                {
                    Id = new Guid("fe84672e-302e-460e-b292-938ac417ccda"),
                    Position = 1,
                },
                new()
                {
                    Id = new Guid("a72d3a45-c580-4152-bf3b-67b33b18a3aa"),
                    Position = 0
                }
            }
        };

        var reorderResponse = await _client.PutAsJsonAsync($"{TodosBaseUrl}/reorder", requestBody);

        Assert.Equal(HttpStatusCode.NoContent, reorderResponse.StatusCode);
    }

    [Fact]
    public async Task ReorderTodos_ReturnsBadRequest_WhenDuplicateTodoIds()
    {
        await SeedDatabaseAsync();
        
        var requestBody = new ReorderTodosRequest()
        {
            Todos = new List<TodoPosition>()
            {
                new()
                {
                    Id = new Guid("fe84672e-302e-460e-b292-938ac417ccda"),
                    Position = 1,
                },
                new()
                {
                    Id = new Guid("fe84672e-302e-460e-b292-938ac417ccda"),
                    Position = 0
                }
            }
        };

        var reorderResponse = await _client.PutAsJsonAsync($"{TodosBaseUrl}/reorder", requestBody);
        
        Assert.Equal(HttpStatusCode.BadRequest, reorderResponse.StatusCode);
    }
    
    [Fact]
    public async Task ReorderTodos_ReturnsBadRequest_WhenDuplicatePositions()
    {
        await SeedDatabaseAsync();
        
        var requestBody = new ReorderTodosRequest()
        {
            Todos = new List<TodoPosition>()
            {
                new()
                {
                    Id = new Guid("fe84672e-302e-460e-b292-938ac417ccda"),
                    Position = 0,
                },
                new()
                {
                    Id = new Guid("a72d3a45-c580-4152-bf3b-67b33b18a3aa"),
                    Position = 0
                }
            }
        };

        var reorderResponse = await _client.PutAsJsonAsync($"{TodosBaseUrl}/reorder", requestBody);
        
        Assert.Equal(HttpStatusCode.BadRequest, reorderResponse.StatusCode);
    }
    
    [Fact]
    public async Task ReorderTodos_ReturnsBadRequest_WhenNegativePosition()
    {
        await SeedDatabaseAsync();
        
        var requestBody = new ReorderTodosRequest()
        {
            Todos = new List<TodoPosition>()
            {
                new()
                {
                    Id = new Guid("fe84672e-302e-460e-b292-938ac417ccda"),
                    Position = -1,
                },
                new()
                {
                    Id = new Guid("fe84672e-302e-460e-b292-938ac417ccda"),
                    Position = 0
                }
            }
        };

        var reorderResponse = await _client.PutAsJsonAsync($"{TodosBaseUrl}/reorder", requestBody);
        
        Assert.Equal(HttpStatusCode.BadRequest, reorderResponse.StatusCode);
    }

    [Fact]
    public async Task ReorderTodos_ReturnsBadRequest_IfTodoDoesNotExist()
    {
        await SeedDatabaseAsync();
        
        var requestBody = new ReorderTodosRequest()
        {
            Todos = new List<TodoPosition>()
            {
                new()
                {
                    Id = new Guid("0acd6977-89ac-40da-a069-8abd63f4feab"),
                    Position = 1,
                },
                new()
                {
                    Id = new Guid("fe84672e-302e-460e-b292-938ac417ccda"),
                    Position = 0
                }
            }
        };

        var response = await _client.PutAsJsonAsync($"{TodosBaseUrl}/reorder", requestBody);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}