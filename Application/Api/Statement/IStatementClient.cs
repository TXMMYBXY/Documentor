using Documentor.Application.Api.Statement.Dtos;
using Documentor.Core.Models;

namespace Documentor.Application.Api.Statement;

public interface IStatementClient
{
    Task<PagedStatementDto> GetStatementsAsync(StatementFilterDto filter);
    Task<bool> ChangeTemplateStatusAsync(int templateId);
    Task UpdateTemplateAsync(int templateId, UpdateTemplateDto templateDto);
    Task CreateTemplateAsync(CreateTemplateDto templateDto);
    Task DeleteTemplateAsync(DeleteTemplateDto templateDto);
}