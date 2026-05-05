using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Document.Dtos;

public class PagedDocumentDto
{
    [JsonPropertyName("documents")]
    public ICollection<DocumentDto>? Documents { get; set; }
    
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }
}