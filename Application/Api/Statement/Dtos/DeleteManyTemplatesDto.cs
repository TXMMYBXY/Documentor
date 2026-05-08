using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Statement.Dtos;

public class DeleteManyTemplatesDto
{
    [JsonPropertyName("templateIds")]
    public List<int> TemplateIds { get; set; }
}