using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Authorization.Dtos;

public class RefreshTokenToLoginRequestDto
{
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
}