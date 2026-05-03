using System.IO;
using Documentor.Core.Models;

namespace Documentor.Application.Api.Document;

public interface IDocumentClient
{
    Task<DownloadFileResult> DownloadDocumentAsync(int documentId);
}
