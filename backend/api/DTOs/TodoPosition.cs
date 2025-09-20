using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

public class TodoPosition
{
    [Required]
    public Guid Id { get; init; }
    
    [Required]
    public int Position { get; init; }
}