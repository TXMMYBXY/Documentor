using Documentor.Core.Enums;
using Documentor.Core.Models.Template;

namespace Documentor.Application.Api.Statement.Dtos;

public class TemplateFilterDto
{
    public TemplateSortField? SortBy { get; set; }
    public bool Descending { get; set; }
    
    public TemplateType Type { get; set; }
    
    public string? Title { get; set; }
    public int? CreatedBy { get; set; } 
    public DateTime? CreatedAtEarlier { get; set; }
    public DateTime? CreatedAtLater { get; set; }
    
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
}