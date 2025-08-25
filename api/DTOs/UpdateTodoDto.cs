namespace api.DTOs;

public class UpdateTodoDto
{
    public string? Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool? IsCompleted { get; set; }
    
    public int? Position { get; set; }
}