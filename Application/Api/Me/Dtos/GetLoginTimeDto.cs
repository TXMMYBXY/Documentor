using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Me.Dtos;

public class GetLoginTimeDto
{
    [JsonPropertyName("loginTime")]
    public DateTime? LoginTime { get; set; }
}