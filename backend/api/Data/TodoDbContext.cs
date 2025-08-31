using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();
}