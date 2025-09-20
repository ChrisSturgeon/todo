using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

public class TodosResponse
{
    [Required]
    public IEnumerable<TodoResponse> Items { get; set; } = new List<TodoResponse>();
    public int TotalCount { get; set; }
}