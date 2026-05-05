using System.Text.Json.Serialization;
using Documentor.Application.Api.Authorization.Dtos;

namespace Documentor.Application.Api.Admin.Dtos.User;

public class TemplateOwnerDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("fullName")]
    public string FullName { get; set; }
    
    [JsonPropertyName("email")]
    public string Email { get; set; }
    
    [JsonPropertyName("role")]
    public RoleDto Role { get; set; }
}