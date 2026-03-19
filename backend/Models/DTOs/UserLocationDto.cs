using System.Text.Json.Serialization;

namespace Enhanzer.Api.Models.DTOs;

public class UserLocationDto
{
    [JsonPropertyName("Location_Code")]
    public string Location_Code { get; set; } = string.Empty;

    [JsonPropertyName("Location_Name")]
    public string Location_Name { get; set; } = string.Empty;
}
