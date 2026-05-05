using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Statement.Dtos;

public class CreateTemplateDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
    
    [JsonPropertyName("filePath")]
    public string FilePath { get; set; } = string.Empty;

}