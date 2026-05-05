using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Statement.Dtos;

public class DynamicFieldInfoDto
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = null!;
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;
    [JsonPropertyName("type")]
    public string Type { get; set; } = "string";
    [JsonPropertyName("required")]
    public bool Required { get; set; }
    [JsonPropertyName("options")]
    public List<string>? Options { get; set; }
}