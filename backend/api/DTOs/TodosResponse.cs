namespace api.DTOs;

public class TodosResponse
{
    public IEnumerable<TodoResponse> Items { get; set; } = new List<TodoResponse>();
    public int TotalCount { get; set; }
}