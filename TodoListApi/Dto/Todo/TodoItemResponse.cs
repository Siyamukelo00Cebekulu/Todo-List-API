using TodoListApi.Models;

namespace TodoListApi.Dto.Todo;

public class TodoItemResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public TodoPriority Priority { get; set; }
}
