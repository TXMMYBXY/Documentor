using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Authorization.Dtos;
public class RefreshTokenDto
{
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
    
    [JsonPropertyName("expiresAt")]
    public DateTimeOffset? ExpiresAt { get; set; }
}
