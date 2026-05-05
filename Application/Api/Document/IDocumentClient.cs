using System.Threading.Tasks;
using Documentor.Application.Api.Document.Dtos;
using Documentor.Application.Api.Statement.Dtos;
using Documentor.Core.Models;

namespace Documentor.Application.Api.Document;

public interface IDocumentClient
{
    Task<DownloadFileResult> DownloadDocumentAsync(int documentId);
    Task<PagedDocumentDto?> GetDocumentsAsync(DocumentFilterDto filter);
    Task DeleteDocumentByIdAsync(int selectedDocumentId);
}
