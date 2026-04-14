namespace TodoListApi.Dto.Todo;

public class TodoQueryParameters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool? IsComplete { get; set; }
    public string? Search { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "desc";
}
