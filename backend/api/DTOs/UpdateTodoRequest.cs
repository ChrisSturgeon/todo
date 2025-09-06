namespace api.DTOs;

public class UpdateTodoRequest
{
    public string? Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool? Completed { get; set; }
}