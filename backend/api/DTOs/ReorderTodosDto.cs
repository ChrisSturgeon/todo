namespace api.DTOs;

public class ReorderTodosDto
{
    public IEnumerable<TodoReorderDto> Todos { get; set; } = new List<TodoReorderDto>();
}