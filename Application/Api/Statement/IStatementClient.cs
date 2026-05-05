using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Documentor.Application.Api.Statement.Dtos;

namespace Documentor.Application.Api.Statement;

public interface IStatementClient
{
    Task<PagedStatementDto> GetTemplateAsync(StatementFilterDto filter);
    Task<bool> ChangeTemplateStatusAsync(int templateId);
    Task UpdateTemplateAsync(int templateId, UpdateTemplateDto templateDto);
    Task CreateTemplateAsync(CreateTemplateDto templateDto);
    Task DeleteTemplateAsync(int templateId);
    Task DeleteManyTemplateAsync(DeleteManyTemplatesDto manyTemplatesDto);
    Task<Stream> DownloadTemplateAsync(int templateId);
    Task<IReadOnlyList<DynamicFieldInfoDto>> ExtractFieldsAsync(int templateId);

    Task CreateTask(CreateTaskRequestDto dto);
}