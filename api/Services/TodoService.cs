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
        return await _repository.GetAllTodosAsync();
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
            Position = currentTodos.Count() + 1,
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
}