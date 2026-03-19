using System.ComponentModel.DataAnnotations;

namespace Enhanzer.Api.Models.DTOs;

public class LoginRequestDto
{
    [Required]
    [RegularExpression(@"^(admin|[^@\s]+@[^@\s]+\.[^@\s]+)$", ErrorMessage = "Email must be valid or use admin.")]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
