using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Authorization.Dtos;

public class RefreshTokenResponseDto
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
    [JsonPropertyName("expiresAt")]
    public string ExpiresAt { get; set; }
}