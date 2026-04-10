using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Authorization.Dtos;

public class RefreshTokenToLoginResponseDto
{
    [JsonPropertyName("isAllowed")]
    public bool IsAllowed { get; set; }
}