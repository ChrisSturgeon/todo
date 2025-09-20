namespace api.DTOs;

public class ReorderTodosRequest
{
    public IEnumerable<TodoPosition> Todos { get; init; } = new List<TodoPosition>();
}