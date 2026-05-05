using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Authorization.Dtos.Requests;

public class RefreshTokenToLoginRequestDto
{
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
}