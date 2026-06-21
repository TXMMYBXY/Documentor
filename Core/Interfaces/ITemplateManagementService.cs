using Documentor.Application.Api.Statement.Dtos;
using Documentor.Core.Enums;
using Documentor.Core.Models;
using Documentor.Core.Models.Template;

namespace Documentor.Core.Interfaces;

public interface ITemplateManagementService
{
    Task<PagedResult<TemplateListItemModel>> GetTemplatesAsync(TemplateFilterModel filter);
    Task<bool> ChangeStatusAsync(int statementId);
    Task DeleteTemplateAsync(int statementId);
    Task UpdateStatementTemplateAsync(int templateId, string? title, string? filePath);
    Task CreateStatementAsync(CreateTemplateModel templateModel);
    Task DownloadTemplateAsync(int templateId, string savePath);
    Task<IReadOnlyList<DynamicFieldInfoModel>> ExtractFieldsAsync(int templateId);
    Task DeleteTemplatesAsync(List<int> templateIds);

    Task<IReadOnlyList<LookupItemModel>> GetTemplatesAsync();
    
    Task CreateTaskAsync(int templateId, TemplateType templateType, Dictionary<string, object> data);
}