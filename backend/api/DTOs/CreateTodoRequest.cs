namespace api.DTOs;

public class CreateTodoRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}