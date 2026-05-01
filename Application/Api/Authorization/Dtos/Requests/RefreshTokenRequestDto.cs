using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Authorization.Dtos.Requests;

public class RefreshTokenRequestDto
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}