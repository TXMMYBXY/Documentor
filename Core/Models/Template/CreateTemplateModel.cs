using Documentor.Core.Enums;

namespace Documentor.Core.Models.Template;

public class CreateTemplateModel
{
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public TemplateType Type { get; set; }
    public string FilePath { get; set; } = string.Empty;
}