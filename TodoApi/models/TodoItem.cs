namespace TodoApi;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public bool IsCompleted { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
