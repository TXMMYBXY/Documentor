using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Authorization.Dtos;

public class AccessTokenRequestDto
{
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
}