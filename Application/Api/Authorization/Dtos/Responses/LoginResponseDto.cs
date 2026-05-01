using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Authorization.Dtos.Responses;
public class LoginResponseDto
{
    [JsonPropertyName("access")]
    public AccessTokenDto? Access { get; set; }
    
    [JsonPropertyName("refresh")]
    public RefreshTokenDto? Refresh { get; set; }
    
    [JsonPropertyName("userInfo")]
    public UserInfoDto? UserInfo { get; set; }
}
