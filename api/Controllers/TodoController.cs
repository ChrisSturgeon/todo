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
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetAllTodos()
    {
        var todos = await _service.GetAllTodosAsync();
        return Ok(todos.Select(t => new TodoDto()
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            Completed = t.Completed,
            Position = t.Position,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoDto?>> GetTodoById(Guid id)
    {
        var todo = await _service.GetTodoByIdAsync(id);

        if (todo is null)
        {
            return NotFound();
        }

        return Ok(new TodoDto
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
    public async Task<ActionResult<TodoDto>> CreateTodo([FromBody] CreateTodoDto dto)
    {
        var todo = await _service.CreateTodoAsync(dto.Name, dto.Description ?? string.Empty);

        return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, new TodoDto()
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
    public async Task<ActionResult> UpdateTodo(Guid id, [FromBody] UpdateTodoDto dto)
    {
        var updatedTodo = await _service.UpdateTodoAsync(id, dto.Name, dto.Description, dto.IsCompleted);

        return updatedTodo ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTodo(Guid id)
    {
        var deleted = await _service.DeleteTodoAsync(id);

        return deleted ? NoContent() : NotFound();
    }

    [HttpPut("reorder")]
    public async Task<ActionResult<IEnumerable<TodoDto>>> ReorderTodos(
        [FromBody] IEnumerable<TodoReorderDto> reorderDtos)
    {
        if (reorderDtos is null || !reorderDtos.Any())
        {
            return BadRequest("No todos provided.");
        }

        var updatedTodos = await _service.ReorderTodosAsync(reorderDtos);

        var result = updatedTodos.Select(t => new Todo
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            Completed = t.Completed,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        });

        return Ok(result);
    }
}