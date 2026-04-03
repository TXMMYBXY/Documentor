using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Admin.Dtos.Department;

public class CreateDepartmentDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}