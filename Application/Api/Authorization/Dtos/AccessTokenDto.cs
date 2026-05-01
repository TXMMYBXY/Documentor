using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Authorization.Dtos;

public class AccessTokenDto
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }
    
    [JsonPropertyName("expiresAt")]
    public DateTimeOffset ExpiresAt { get; set; }
    
    [JsonPropertyName("tokenType")]
    public string TokenType { get; set; } = "Bearer";
}