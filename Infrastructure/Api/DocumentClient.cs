using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Documentor.Application.Api.Document;
using Documentor.Application.Api.Models;
using Documentor.Core.Models;
using Microsoft.Extensions.Options;

namespace Documentor.Infrastructure.Api;

public class DocumentClient : IDocumentClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    
    public DocumentClient(
        HttpClient httpClient, 
        IOptions<DocumentFlowApi> documentFlowApi) 
    {
        _httpClient = httpClient;
        _baseUrl = documentFlowApi.Value.Domain;
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
}