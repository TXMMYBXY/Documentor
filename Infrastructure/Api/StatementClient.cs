using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Documentor.Application.Api.Models;
using Documentor.Application.Api.Statement;
using Documentor.Application.Api.Statement.Dtos;
using Microsoft.Extensions.Options;

namespace Documentor.Infrastructure.Api;

public class StatementClient : GeneralClient, IStatementClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    
    public StatementClient(HttpClient httpClient, IOptions<DocumentFlowApi> documentFlowApi) : base(httpClient, documentFlowApi)
    {
        _httpClient = httpClient;
        _baseUrl = documentFlowApi.Value.Domain;
    }

    public async Task<PagedStatementDto> GetStatementsAsync(StatementFilterDto filter)
    {
        var query = _BuildTemplateQuery(filter);
        
        return await GetResponseAsync<PagedStatementDto>($"statement-template{query}");
    }

    public async Task<bool> ChangeTemplateStatusAsync(int templateId)
    {
        return await PatchResponseAsync<object, bool>(null, $"statement-template/{templateId}/change-template-status");
    }

    public async Task UpdateTemplateAsync(int templateId, UpdateTemplateDto templateDto)
    {
        await PatchResponseAsync<UpdateTemplateDto, object>(templateDto, $"statement-template/{templateId}");
    }

    public async Task CreateTemplateAsync(CreateTemplateDto templateDto)
    {
        using var form = new MultipartFormDataContent();

        form.Add(new StringContent(templateDto.Title), "title");
        form.Add(new StringContent(templateDto.IsActive.ToString().ToLowerInvariant()), "isActive");

        await using var fileStream = File.OpenRead(templateDto.FilePath);
        
        using var streamContent = new StreamContent(fileStream);
        
        streamContent.Headers.ContentType =
            new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

        form.Add(streamContent, "file", Path.GetFileName(templateDto.FilePath));

        var response = await _httpClient.PostAsync($"{_baseUrl}statement-template", form);
        
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteTemplateAsync(int templateId)
    {
        await DeleteResponseAsync<object>( $"statement-template/{templateId}");
    }

    public async Task DeleteManyTemplateAsync(DeleteManyTemplatesDto manyTemplatesDto)
    {
        await MultipleDeletionResponseAsync<DeleteManyTemplatesDto, object>(manyTemplatesDto, "statement-template");
    }

    public async Task<Stream> DownloadTemplateAsync(int templateId)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}statement-template/{templateId}/download");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync();
    }

    private static string _BuildTemplateQuery(StatementFilterDto filter)
    {
        var parameters = new List<string>();

        if (!string.IsNullOrWhiteSpace(filter.Title))
            parameters.Add($"Title={Uri.EscapeDataString(filter.Title)}");

        if (filter.CreatedBy.HasValue)
            parameters.Add($"CreatedBy={filter.CreatedBy.Value}");

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