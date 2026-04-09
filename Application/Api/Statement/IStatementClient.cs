using System.IO;
using Documentor.Application.Api.Statement.Dtos;
using Documentor.Core.Models;

namespace Documentor.Application.Api.Statement;

public interface IStatementClient
{
    Task<PagedStatementDto> GetStatementsAsync(StatementFilterDto filter);
    Task<bool> ChangeTemplateStatusAsync(int templateId);
    Task UpdateTemplateAsync(int templateId, UpdateTemplateDto templateDto);
    Task CreateTemplateAsync(CreateTemplateDto templateDto);
    Task DeleteTemplateAsync(int templateId);
    Task DeleteManyTemplateAsync(DeleteManyTemplatesDto manyTemplatesDto);
    Task<Stream> DownloadTemplateAsync(int templateId);
    Task<IReadOnlyList<DynamicFieldInfoDto>> ExtractFieldsAsync(int templateId);

    Task CreateTask(CreateTaskRequestDto dto);
}