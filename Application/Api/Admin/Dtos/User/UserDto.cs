using System.Text.Json.Serialization;
using Documentor.Application.Api.Admin.Dtos.Department;
using Documentor.Application.Api.Authorization.Dtos;
using Documentor.Core.Enums;

namespace Documentor.Application.Api.Admin.Dtos.User;

public class UserDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("fullName")]
    public string FullName { get; set; }
    
    [JsonPropertyName("email")]
    public string Email { get; set; }
    
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
    
    [JsonPropertyName("department")]
    public DepartmentCleanDto Department { get; set; }
    
    [JsonPropertyName("role")]
    public Role Role { get; set; }
}