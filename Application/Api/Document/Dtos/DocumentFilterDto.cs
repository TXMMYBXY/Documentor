using Documentor.Core.Models.Document;

namespace Documentor.Application.Api.Document.Dtos;

public class DocumentFilterDto
{
    public DocumentSortField? SortBy { get; set; }
    public bool Descending { get; set; }
    
    public string? Title { get; set; }
    public int? TemplateId { get; set; }
    public DateTime? CreatedAtEarlier { get; set; }
    public DateTime? CreatedAtLater { get; set; }
    
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
}