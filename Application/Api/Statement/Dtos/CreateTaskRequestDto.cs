using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Documentor.Core.Enums;

namespace Documentor.Application.Api.Statement.Dtos;

public class CreateTaskRequestDto
{
    [JsonPropertyName("templateId")]
    public int TemplateId { get; set; }
    
    [JsonPropertyName("templateType")]
    [EnumDataType(typeof(TemplateType))]
    public TemplateType TemplateType { get; set; }
    
    [JsonPropertyName("data")]
    public Dictionary<string, object> Data { get; set; } = new();
}