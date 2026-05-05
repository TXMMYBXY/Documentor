namespace Documentor.Application.Api.Document.Dtos;

public class DocumentFilterDto
{
    public string? SortByField { get; set; }
    public bool Descending { get; set; }
    
    public string? Title { get; set; }
    public int? TemplateId { get; set; }
    public DateTime? CreatedAtEarlier { get; set; }
    public DateTime? CreatedAtLater { get; set; }
    
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
}