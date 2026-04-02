using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Admin.Dtos.Department;

public class DeleteDepartmentDto
{
    [JsonPropertyName("departmentId")]
    public int DepartmentId { get; set; }
}