using api.DTOs;
using api.Models;
using api.Repositories.Interfaces;
using api.Services.Interfaces;

namespace api.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<Todo>> GetAllTodosAsync()
    {
        var allTodos = await _repository.GetAllTodosAsync();

        return allTodos.OrderBy(t => t.Position);
    }

    public async Task<Todo?> GetTodoByIdAsync(Guid id)
    {
        return await _repository.GetTodoByIdAsync(id);
    }

    public async Task<Todo> CreateTodoAsync(string name, string? description)
    {
        var currentTodos = await this.GetAllTodosAsync();
        
        var newTodo = new Todo
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Completed = false,
            Position = currentTodos.Count(),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        await _repository.AddTodoAsync(newTodo);

        return newTodo;
    }

    public async Task<bool> UpdateTodoAsync(Guid id, string? name, string? description, bool? isCompleted)
    {
        var todo = await this.GetTodoByIdAsync(id);

        if (todo is null)
        {
            return false;
        }

        todo.Name = name ?? todo.Name;
        todo.Description = description ?? todo.Description;
        todo.Completed = isCompleted ?? todo.Completed;
        todo.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.UpdateTodoAsync(todo);
        return true;
    }

    public async Task<bool> DeleteTodoAsync(Guid id)
    {
        var todo = await this.GetTodoByIdAsync(id);

        if (todo is null)
        {
            return false;
        }

        await _repository.DeleteTodoAsync(todo);
        return true;
    }

    public async Task<IEnumerable<Todo>> ReorderTodosAsync(IEnumerable<TodoReorderDto> reorderDtos)
    {
        var allTodos = await GetAllTodosAsync();

        foreach (var dto in reorderDtos)
        {
            var todo = allTodos.FirstOrDefault(t => t.Id == dto.Id);

            if (todo is null) continue;
            
            todo.Position = dto.Position;
            todo.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await _repository.UpdateTodoPositionsAsync(allTodos);

        return allTodos.OrderBy(t => t.Position);
    }
}