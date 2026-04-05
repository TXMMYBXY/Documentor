using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Statement.Dtos;

public class DeleteTemplateDto
{
    [JsonPropertyName("templateId")]
    public int TemplateId { get; set; }
}