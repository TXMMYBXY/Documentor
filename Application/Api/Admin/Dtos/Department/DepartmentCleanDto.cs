using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Admin.Dtos.Department;

public class DepartmentCleanDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}