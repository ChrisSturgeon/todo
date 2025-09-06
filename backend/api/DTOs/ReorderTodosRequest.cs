namespace api.DTOs;

public class ReorderTodosRequest
{
    public IEnumerable<ReorderTodoDto> Todos { get; init; } = new List<ReorderTodoDto>();
}