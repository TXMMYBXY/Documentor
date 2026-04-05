using Documentor.Core.Models;
using Documentor.Core.Models.Statement;

namespace Documentor.Core.Interfaces;

public interface IStatementManagementService
{
    Task<PagedResult<StatementListItemModel>> GetStatementsAsync(StatementFilterModel filter);
    Task<bool> ChangeStatusAsync(int statementId);
    Task DeleteStatementAsync(int statementId);
    
    Task CreateStatementAsync(CreateStatementTemplateModel templateModel);
    Task DownloadStatementTemplateAsync(int templateId, string savePath);
}