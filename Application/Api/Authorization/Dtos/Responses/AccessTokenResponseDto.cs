using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Authorization.Dtos.Responses;

public class AccessTokenResponseDto
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }
    
    [JsonPropertyName("expiresAt")]
    public DateTimeOffset ExpiresAt { get; set; }
    
    [JsonPropertyName("tokenType")]
    public string TokenType { get; set; } = "Bearer";
}