using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Documentor.Application.Api.Document;
using Documentor.Application.Api.Document.Dtos;
using Documentor.Core.Models;

namespace Documentor.Infrastructure.Api;

public class DocumentClient : GeneralClient, IDocumentClient
{
    private readonly HttpClient _httpClient;
    
    public DocumentClient(HttpClient httpClient):base(httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DownloadFileResult> DownloadDocumentAsync(int documentId)
    {
        var response = await _httpClient.GetAsync(
            $"document/{documentId}/download",
            HttpCompletionOption.ResponseHeadersRead);

        response.EnsureSuccessStatusCode();

        var fileName = _GetFileNameFromResponse(response, documentId);
        var stream = await response.Content.ReadAsStreamAsync();

        return new DownloadFileResult(response, fileName, stream);
    }

    public async Task<PagedDocumentDto?> GetDocumentsAsync(DocumentFilterDto filter)
    {
        var query = _BuildDocumentsQuery(filter);
        
        return await GetResponseAsync<PagedDocumentDto>($"document{query}");
    }

    public async Task DeleteDocumentByIdAsync(int selectedDocumentId)
    {
        await DeleteResponseAsync<object>($"document/{selectedDocumentId}");
    }

    private static string _GetFileNameFromResponse(HttpResponseMessage response, int documentId)
    {
        var cd = response.Content.Headers.ContentDisposition;
        
        if (cd == null && response.Content.Headers.TryGetValues("Content-Disposition", out var values))
        {
            if (ContentDispositionHeaderValue.TryParse(values.FirstOrDefault(), out var parsed))
                cd = parsed;
        }

        var name = cd?.FileNameStar ?? cd?.FileName;

        if (string.IsNullOrWhiteSpace(name))
            return $"document_{documentId}.docx";

        return name.Trim('"');
    }
    
    private static string _BuildDocumentsQuery(DocumentFilterDto filter)
    {
        var parameters = new List<string>();
        
        if (!string.IsNullOrWhiteSpace(filter.SortByField))
            parameters.Add($"SortByField={Uri.EscapeDataString(filter.SortByField)}");
        
        if (filter.Descending)
            parameters.Add($"Descending=true");

        if (!string.IsNullOrWhiteSpace(filter.Title))
            parameters.Add($"Title={Uri.EscapeDataString(filter.Title)}");

        if (filter.TemplateId.HasValue)
            parameters.Add($"TemplateId={filter.TemplateId.Value}");
        
        if (filter.CreatedAtLater.HasValue)
        {
            var fromDate = filter.CreatedAtLater.Value.Date;
            parameters.Add($"CreatedAtLater={Uri.EscapeDataString(fromDate.ToString("O"))}");
        }

        if (filter.CreatedAtEarlier.HasValue)
        {
            var toDate = filter.CreatedAtEarlier.Value.Date.AddDays(1).AddTicks(-1);
            parameters.Add($"CreatedAtEarlier={Uri.EscapeDataString(toDate.ToString("O"))}");
        }

        if (filter.PageSize.HasValue)
            parameters.Add($"PageSize={filter.PageSize.Value}");

        if (filter.PageNumber.HasValue)
            parameters.Add($"PageNumber={filter.PageNumber.Value}");

        if (parameters.Count == 0)
            return string.Empty;

        return "?" + string.Join("&", parameters);
    }
}