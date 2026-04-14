using System.ComponentModel.DataAnnotations;

namespace TodoListApi.Dto.Auth;

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
