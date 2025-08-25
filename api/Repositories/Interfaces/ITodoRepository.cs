using api.Models;

namespace api.Repositories.Interfaces;

public interface ITodoRepository
{
    Task<IEnumerable<Todo>> GetAllTodosAsync();
    Task<Todo?> GetTodoByIdAsync(Guid id);
    Task AddTodoAsync(Todo todo);
    Task UpdateTodoAsync(Todo todo);
    Task DeleteTodoAsync(Todo todo);
    Task UpdateTodoPositionsAsync(IEnumerable<Todo> todos);
}