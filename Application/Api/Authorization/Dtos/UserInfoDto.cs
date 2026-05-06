using System.Text.Json.Serialization;
using Documentor.Core.Enums;

namespace Documentor.Application.Api.Authorization.Dtos;
public class UserInfoDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("email")]
    public string Email { get; set; }
    
    [JsonPropertyName("department")]
    public string Department { get; set; }
    
    [JsonPropertyName("role")]
    public Role Role { get; set; }
}
