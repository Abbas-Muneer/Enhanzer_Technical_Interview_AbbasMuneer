namespace Enhanzer.Api.Models.Entities;

public class LocationDetail
{
    public int Id { get; set; }
    public string Location_Code { get; set; } = string.Empty;
    public string Location_Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
