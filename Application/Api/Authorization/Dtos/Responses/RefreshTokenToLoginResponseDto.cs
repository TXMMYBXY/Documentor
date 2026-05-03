using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Authorization.Dtos.Responses;

public class RefreshTokenToLoginResponseDto
{
    [JsonPropertyName("isAllowed")]
    public bool IsAllowed { get; set; }
    
    [JsonPropertyName("accessToken")]
    public AccessTokenDto Access { get; set; }
    
    [JsonPropertyName("refreshToken")]
    public RefreshTokenDto Refresh { get; set; }
}