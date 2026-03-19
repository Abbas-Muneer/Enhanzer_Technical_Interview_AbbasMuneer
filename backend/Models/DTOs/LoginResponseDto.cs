namespace Enhanzer.Api.Models.DTOs;

public class LoginResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<LocationDto> Locations { get; set; } = [];
}
