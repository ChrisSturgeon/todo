using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using api.Repositories.Interfaces;

namespace api.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly TodoDbContext _context;

    public TodoRepository(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Todo>> GetAllTodosAsync()
    {
        return await _context.Todos.ToListAsync();
    }

    public async Task<Todo?> GetTodoByIdAsync(Guid id)
    {
        return await _context.Todos.FindAsync(id);
    }

    public async Task<List<Todo>> GetTodosByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _context.Todos
            .Where(t => ids.Contains(t.Id))
            .ToListAsync();
    }

    public async Task AddTodoAsync(Todo todo)
    {
        await _context.Todos.AddAsync(todo);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTodoAsync(Todo todo)
    {
        _context.Todos.Update(todo);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTodoAsync(Todo todo)
    {
        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTodoPositionsAsync(IEnumerable<Todo> todos)
    {
        _context.Todos.UpdateRange(todos);
        await _context.SaveChangesAsync();
    }
}