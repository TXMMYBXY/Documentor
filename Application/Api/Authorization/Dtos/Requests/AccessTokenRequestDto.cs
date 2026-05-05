using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Authorization.Dtos.Requests;

public class AccessTokenRequestDto
{
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
}