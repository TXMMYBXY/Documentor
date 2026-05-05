using Documentor.Core.Enums;

namespace Documentor.Application.Api.Statement.Dtos;

public class StatementFilterDto
{
    public TemplateType Type { get; set; }
    
    public string? Title { get; set; }
    public int? CreatedBy { get; set; } 
    public DateTime? CreatedAtEarlier { get; set; }
    public DateTime? CreatedAtLater { get; set; }
    
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
}