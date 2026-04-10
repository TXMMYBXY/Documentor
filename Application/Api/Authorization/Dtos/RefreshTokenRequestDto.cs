using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Authorization.Dtos;

public class RefreshTokenRequestDto
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}