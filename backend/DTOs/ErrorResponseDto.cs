namespace Enhanzer.Api.DTOs;

public class ErrorResponseDto
{
    public string Message { get; set; } = string.Empty;
    public IDictionary<string, string[]>? Errors { get; set; }
}
