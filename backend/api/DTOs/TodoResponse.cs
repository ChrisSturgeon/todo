using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

public class TodoResponse
{
    [Required]
    public Guid Id { get; init; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Completed { get; set; }
    public int Position { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}