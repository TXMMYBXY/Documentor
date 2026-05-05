namespace Documentor.Core.Models.Statement;

public class DynamicFieldInfoModel
{
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = "string";
    public bool Required { get; set; }
    public List<string>? Options { get; set; }
}