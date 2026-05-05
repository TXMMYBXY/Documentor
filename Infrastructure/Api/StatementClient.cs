using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Documentor.Application.Api.Statement;
using Documentor.Application.Api.Statement.Dtos;

namespace Documentor.Infrastructure.Api;

public class StatementClient : GeneralClient, IStatementClient
{
    private readonly HttpClient _httpClient;
    
    public StatementClient(HttpClient httpClient) : base(httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedStatementDto> GetTemplateAsync(StatementFilterDto filter)
    {
        var query = _BuildTemplateQuery(filter);
        
        return await GetResponseAsync<PagedStatementDto>($"template{query}");
    }

    public async Task<bool> ChangeTemplateStatusAsync(int templateId)
    {
        return await PatchResponseAsync<object, bool>(null, $"template/{templateId}/change-template-status");
    }

    public async Task UpdateTemplateAsync(int templateId, UpdateTemplateDto templateDto)
    {
        var hasTitle = !string.IsNullOrWhiteSpace(templateDto.Title);
        var hasFile = !string.IsNullOrWhiteSpace(templateDto.FilePath);

        if (!hasTitle && !hasFile)
            return;

        using var form = new MultipartFormDataContent();

        if (hasTitle)
            form.Add(new StringContent(templateDto.Title!), "title");

        if (hasFile)
        {
            var fileStream = File.OpenRead(templateDto.FilePath!);
            var streamContent = new StreamContent(fileStream);

            streamContent.Headers.ContentType =
                new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            form.Add(streamContent, "file", Path.GetFileName(templateDto.FilePath));
        }

        var url = $"template/{templateId}/update-template";

        using var request = new HttpRequestMessage(HttpMethod.Patch, url)
        {
            Content = form
        };

        using var response = await _httpClient.SendAsync(request);
        
        response.EnsureSuccessStatusCode();
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

        var response = await _httpClient.PostAsync("template", form);
        
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteTemplateAsync(int templateId)
    {
        await DeleteResponseAsync<object>( $"template/{templateId}");
    }

    public async Task DeleteManyTemplateAsync(DeleteManyTemplatesDto manyTemplatesDto)
    {
        await MultipleDeletionResponseAsync<DeleteManyTemplatesDto, object>(manyTemplatesDto, "template");
    }

    public async Task<Stream> DownloadTemplateAsync(int templateId)
    {
        var response = await _httpClient.GetAsync($"template/{templateId}/download");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync();
    }

    public async Task<IReadOnlyList<DynamicFieldInfoDto>> ExtractFieldsAsync(int templateId)
    {
        return await GetResponseAsync<IReadOnlyList<DynamicFieldInfoDto>>( $"template/{templateId}/extract-fields");
    }

    public async Task  CreateTask(CreateTaskRequestDto dto)
    {
        await PostResponseAsync<CreateTaskRequestDto, object>(dto, "issue/generate");
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