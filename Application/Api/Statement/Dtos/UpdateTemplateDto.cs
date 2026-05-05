using System.Text.Json.Serialization;

namespace Documentor.Application.Api.Statement.Dtos;

public class UpdateTemplateDto
{
    public string Title { get; set; }

    public string? FilePath { get; set; }
}