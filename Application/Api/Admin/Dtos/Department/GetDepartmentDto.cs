using System.Text.Json.Serialization;
using Documentor.Application.Api.Admin.Dtos.User;

namespace Documentor.Application.Api.Admin.Dtos.Department;

public class GetDepartmentDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("employees")]
    public virtual List<EmployeeDto> Employees { get; set; }
}