using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Models;

public class ErrorResponse
{
    [JsonPropertyName("Message")]
    public string Message { get; set; } = string.Empty;
}