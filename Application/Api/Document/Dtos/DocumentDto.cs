using Documentor.Application.Api.Statement.Dtos;
using Documentor.Core.Enums;

namespace Documentor.Application.Api.Document.Dtos;

public class DocumentDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public TemplateType Type { get; set; }
    public TemplateClearDto Template { get; set; }
}