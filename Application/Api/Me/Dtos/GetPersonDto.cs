using System.Text.Json.Serialization;
using Documentor.Application.Api.Models;

namespace Documentor.Application.Api.Me.Dtos;

public class GetPersonDto
{
    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("department")]
    public string? Department { get; set; }
    
    [JsonPropertyName("role")]
    public Role? Role { get; set; }
}