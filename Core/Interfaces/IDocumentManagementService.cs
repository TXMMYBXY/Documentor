using Documentor.Core.Models;
using Documentor.Core.Models.Document;

namespace Documentor.Core.Interfaces;

public interface IDocumentManagementService
{
    Task<PagedResult<DocumentListItemModel>> GetAllDocumentsAsync(DocumentFilterModel filter);
    Task DeleteDocumentAsync(int selectedDocumentId);
    Task DownloadDocumentAsync(int selectedDocumentId, string dialogFileName);
}