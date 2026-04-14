using System.ComponentModel.DataAnnotations;
using TodoListApi.Models;

namespace TodoListApi.Dto.Todo;

public class TodoUpdateRequest
{
    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public bool? IsComplete { get; set; }
    public DateTime? DueDate { get; set; }
    public TodoPriority? Priority { get; set; }
}
