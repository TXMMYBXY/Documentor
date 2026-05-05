using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Authorization.Dtos.Requests;
public class LoginRequestDto
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}
