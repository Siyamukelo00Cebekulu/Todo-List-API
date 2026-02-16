using System.ComponentModel.DataAnnotations;

namespace TodoApi;

public class TodoDto
{
    [Required]
    public string Title { get; set; } = null!;
}
