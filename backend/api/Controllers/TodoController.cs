using api.DTOs;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiExplorerSettings(GroupName = "v1")]
[Route("/api/v1/[controller]")]
[ApiController]
[Produces("application/json")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _service;

    public TodoController(ITodoService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// Get all todos in the system
    /// </summary>
    /// <returns>A list of all the todos</returns>
    /// <response code="200">Returns a list of all todos</response>
    /// <response code="500">Server error while retrieving todos</response>
    [HttpGet]
    [ProducesResponseType(typeof(TodosResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TodosResponse>> GetAllTodos()
    {
        var todos = await _service.GetAllTodosAsync();
        var todosList = todos.ToList();
        var response = new TodosResponse()
        {
            Items = todosList.Select(t => new TodoResponse()
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Completed = t.Completed,
                Position = t.Position,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }),
            TotalCount = todosList.Count,
        };
         
        return Ok(response);
    }

    /// <summary>
    /// Retrieves a specific todo by id
    /// </summary>
    /// <param name="id">The unique identifier of the todo</param>
    /// <returns>The requested todo</returns>
    /// <response code="200">Returns the requested todo</response>
    /// <response code="404">Todo not found</response>
    /// <response code="500">Server error while retrieving the todo</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TodoResponse?>> GetTodoById([FromRoute]Guid id)
    {
        var todo = await _service.GetTodoByIdAsync(id);

        if (todo is null)
        {
            return NotFound();
        }

        return Ok(new TodoResponse
        {
            Id = todo.Id,
            Name = todo.Name,
            Description = todo.Description,
            Completed = todo.Completed,
            Position = todo.Position,
            CreatedAt = todo.CreatedAt,
            UpdatedAt = todo.UpdatedAt
        });
    }

    /// <summary>
    /// Creates a new todo
    /// </summary>
    /// <param name="request">Todo creation details</param>
    /// <returns>The created todo</returns>
    /// <response code="201">The todo was successfully created</response>
    /// <response code="400">The request body failed validation</response>
    /// <response code="500">Server error while retrieving todos</response>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TodoResponse>> CreateTodo([FromBody] CreateTodoRequest request)
    {
        var todo = await _service.CreateTodoAsync(request.Name, request.Description ?? string.Empty);

        return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, new TodoResponse()
        {
            Id = todo.Id,
            Name = todo.Name,
            Description = todo.Description,
            Completed = todo.Completed,
            Position = todo.Position,
            CreatedAt = todo.CreatedAt,
            UpdatedAt = todo.UpdatedAt,
        });
    }

    /// <summary>
    /// Updates a todo
    /// </summary>
    /// <param name="id">The unique identifier of the todo to update</param>
    /// <param name="request">The todo update details</param>
    /// <returns>No content</returns>
    /// <response code="204">The todo was successfully updated</response>
    /// <response code="400">The request body failed validation</response>
    /// <response code="404">The todo to update could not be found</response>
    /// <response code="500">Server error while updating the todo</response>
    [HttpPatch("{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateTodo(Guid id, [FromBody] UpdateTodoRequest request)
    {
        var updatedTodo = await _service.UpdateTodoAsync(id, request.Name, request.Description, request.Completed);

        return updatedTodo ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes a todo
    /// </summary>
    /// <param name="id">The unique identifier of the todo to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">The todo was successfully deleted</response>
    /// <response code="404">The todo to delete could not be found</response>
    /// <response code="500">Server error while deleting the todo</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteTodo(Guid id)
    {
        var deleted = await _service.DeleteTodoAsync(id);

        return deleted ? NoContent() : NotFound();
    }

    /// <summary>
    /// Reorders todos by assigning new positions
    /// </summary>
    /// <param name="request">An enumerable object of the reordered todos dto</param>
    /// <returns>No content</returns>
    /// <response code="204">The todos were successfully reordered</response>
    /// <response code="400">The request body failed validation</response>
    /// <response code="500">Server error while reordering the todos</response>
    [HttpPut("reorder")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ReorderTodos(
        [FromBody] ReorderTodosRequest request)
    {
        var success = await _service.ReorderTodosAsync(request.Todos);

        return success ? NoContent() : BadRequest();
    }
}