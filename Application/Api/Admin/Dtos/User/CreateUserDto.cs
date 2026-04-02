using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Admin.Dtos.User;

public class CreateUserDto
{
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("password")]
    public string Password { get; set; }
    [JsonPropertyName("fullName")]
    public string FullName { get; set; }
    [JsonPropertyName("departmentId")]
    public int DepartmentId { get; set; }
    [JsonPropertyName("roleId")]
    public int RoleId { get; set; }
}