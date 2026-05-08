namespace Documentor.Core.Models.Statement;

public class CreateStatementTemplateModel
{
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string FilePath { get; set; } = string.Empty;
}