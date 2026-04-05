using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Statement.Dtos;

public class CreateTemplateDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
}