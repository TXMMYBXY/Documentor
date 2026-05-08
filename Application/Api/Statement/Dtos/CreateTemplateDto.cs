using System.Text.Json.Serialization;
using Documentor.Core.Enums;

namespace Documentor.Application.Api.Statement.Dtos;

public class CreateTemplateDto
{
    public string Title { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public TemplateType Type { get; set; }
    public bool IsActive { get; set; }

}