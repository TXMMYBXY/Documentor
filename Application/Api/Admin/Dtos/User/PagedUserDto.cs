using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Admin.Dtos.User;

public class PagedUserDto
{
    [JsonPropertyName("users")]
    public List<GetUserDto> Users { get; set; } = new();

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }
}