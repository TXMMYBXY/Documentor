using System.Text.Json.Serialization;
using Documentor.Application.Api.Admin.Dtos.User;

namespace Documentor.Application.Api.Statement.Dtos;

public class GetTemplateDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("createdBy")]
    public int CreatedBy { get; set; }

    [JsonPropertyName("user")]
    public GetUserDto User { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
}