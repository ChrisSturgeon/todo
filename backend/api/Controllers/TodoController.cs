using api.DTOs;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly ITodoService _service;

    public TodoController(ITodoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<TodosResponse>> GetAllTodos()
    {
        var todos = await _service.GetAllTodosAsync();

        var response = new TodosResponse()
        {
            Items = todos.Select(t => new TodoResponse()
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Completed = t.Completed,
                Position = t.Position,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }),
            TotalCount = todos.Count(),
        };
         
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoResponse?>> GetTodoById(Guid id)
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

    [HttpPost]
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

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult> UpdateTodo(Guid id, [FromBody] UpdateTodoRequest request)
    {
        var updatedTodo = await _service.UpdateTodoAsync(id, request.Name, request.Description, request.Completed);

        return updatedTodo ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTodo(Guid id)
    {
        var deleted = await _service.DeleteTodoAsync(id);

        return deleted ? NoContent() : NotFound();
    }

    [HttpPut("reorder")]
    public async Task<ActionResult<IEnumerable<TodoResponse>>> ReorderTodos(
        [FromBody] ReorderTodosRequest request)
    {
        var success = await _service.ReorderTodosAsync(request.Todos);

        return success ? NoContent() : NotFound();
    }
}