using Documentor.Core.Models;
using Documentor.Core.Models.Statement;

namespace Documentor.Core.Interfaces;

public interface IStatementManagementService
{
    Task<PagedResult<StatementListItemModel>> GetStatementsAsync(StatementFilterModel filter);
    Task<bool> ChangeStatusAsync(int statementId);
    Task DeleteStatementAsync(int statementId);
    Task UpdateStatementTemplateAsync(int templateId, string? title, string? filePath);
    Task CreateStatementAsync(CreateStatementTemplateModel templateModel);
    Task DownloadStatementTemplateAsync(int templateId, string savePath);
    Task<IReadOnlyList<DynamicFieldInfoModel>> ExtractFieldsAsync(int templateId);
    
    Task CreateTaskAsync(int templateId, Dictionary<string, object> data);
}