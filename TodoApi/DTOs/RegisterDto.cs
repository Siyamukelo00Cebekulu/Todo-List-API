using System.ComponentModel.DataAnnotations;

namespace TodoApi;

public class RegisterDto
{
    [Required, MinLength(4)]
    public string Username { get; set; } = null!;

    [Required, MinLength(6)]
    public string Password { get; set; } = null!;
}
