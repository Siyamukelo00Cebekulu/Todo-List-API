using System.ComponentModel.DataAnnotations;
using TodoListApi.Models;

namespace TodoListApi.Dto.Todo;

public class TodoCreateRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public bool IsComplete { get; set; }
    public DateTime? DueDate { get; set; }
    public TodoPriority Priority { get; set; } = TodoPriority.Medium;
}
