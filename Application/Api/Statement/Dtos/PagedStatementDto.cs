using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Statement.Dtos;

public class PagedStatementDto
{
    [JsonPropertyName("templates")]
    public List<GetTemplateDto> Templates { get; set; }
    
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}