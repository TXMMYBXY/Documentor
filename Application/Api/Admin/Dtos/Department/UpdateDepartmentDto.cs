using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Admin.Dtos.Department;

public class UpdateDepartmentDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("employeesIds")]
    public List<int>? EmployeesIds { get; set; }
}