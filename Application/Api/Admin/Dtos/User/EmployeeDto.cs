using System.Text.Json.Serialization;
using Documentor.Application.Api.Models;
using Documentor.Core.Enums;

namespace Documentor.Application.Api.Admin.Dtos.User;

public class EmployeeDto
{
    [JsonPropertyName("fullName")]
    public string FullName { get; set; }
    
    [JsonPropertyName("email")]
    public string Email { get; set; }
    
    [JsonPropertyName("role")]
    public Role Role { get; set; }
}