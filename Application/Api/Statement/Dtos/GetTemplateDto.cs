using System.Text.Json.Serialization;
using Documentor.Application.Api.Admin.Dtos.User;
using Documentor.Core.Enums;

namespace Documentor.Application.Api.Statement.Dtos;

public class GetTemplateDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("createdBy")]
    public TemplateOwnerDto CreatedBy { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
    
    [JsonPropertyName("type")]
    public TemplateType Type { get; set; }
}