using System.Net;
using System.Net.Http.Json;
using api.DTOs;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Testing;

namespace api.IntegrationTests;

public class TodoControllerTests : IClassFixture<TodoApiFactory>
{
    private readonly HttpClient _client;

    public TodoControllerTests(TodoApiFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GetAllTodos_ReturnsSuccessAndCorrectContentType()
    {
        var response = await _client.GetAsync("/api/Todo");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task GetAllTodos_ReturnsExpectedResponseBody()
    {
        var responseBody = await _client.GetFromJsonAsync<TodosResponse>("/api/todo");

        Assert.NotNull(responseBody);
        Assert.Equal(2, responseBody.Items.Count());
        Assert.Equal(2, responseBody.TotalCount);
    }

    [Fact]
    public async Task GetTodoReturns_ExistingTodo()
    {
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