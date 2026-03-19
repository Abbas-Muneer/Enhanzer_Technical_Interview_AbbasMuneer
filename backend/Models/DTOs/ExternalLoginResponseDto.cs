namespace Enhanzer.Api.Models.DTOs;

public class ExternalLoginResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<UserLocationDto> UserLocations { get; set; } = [];
    public string RawResponse { get; set; } = string.Empty;
}
