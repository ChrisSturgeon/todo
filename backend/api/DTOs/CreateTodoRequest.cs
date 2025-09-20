using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

public class CreateTodoRequest
{
    [Required]
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}