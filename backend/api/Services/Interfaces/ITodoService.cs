using api.DTOs;
using api.Models;

namespace api.Services.Interfaces;

public interface ITodoService
{
    Task<IEnumerable<Todo>> GetAllTodosAsync();
    Task<Todo?> GetTodoByIdAsync(Guid id);
    Task<Todo> CreateTodoAsync(string name, string description);
    Task<bool> UpdateTodoAsync(Guid id, string? name, string? description, bool? isCompleted);
    Task<bool> DeleteTodoAsync(Guid id);
    Task<IEnumerable<Todo>> ReorderTodosAsync(IEnumerable<TodoReorderDto> reorderDtos);
}