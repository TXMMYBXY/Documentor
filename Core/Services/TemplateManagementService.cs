using System.IO;
using AutoMapper;
using Documentor.Application.Api.Statement;
using Documentor.Application.Api.Statement.Dtos;
using Documentor.Core.Enums;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.Template;

namespace Documentor.Core.Services;

public class TemplateManagementService : ITemplateManagementService
{
    private readonly ITemplateClient _templateClient;
    private readonly IMapper _mapper;

    public TemplateManagementService(ITemplateClient templateClient, IMapper mapper)
    {
        _templateClient = templateClient;
        _mapper = mapper;
    }

    public async Task<PagedResult<TemplateListItemModel>> GetTemplatesAsync(TemplateFilterModel filter)
    {
        var filterDto = _mapper.Map<TemplateFilterDto>(filter);
        var response = await _templateClient.GetTemplateAsync(filterDto);

        if (response == null)
        {
            return new PagedResult<TemplateListItemModel>();
        }

        return new PagedResult<TemplateListItemModel>
        {
            Items = _mapper.Map<IReadOnlyList<TemplateListItemModel>>(response.Templates),
            TotalCount = response.TotalCount,
            PageSize = response.PageSize,
            CurrentPage = response.CurrentPage,
            TotalPages = response.TotalPages
        };
    }

    public async Task<bool> ChangeStatusAsync(int statementId)
    {
        return await _templateClient.ChangeTemplateStatusAsync(statementId);
    }

    public async Task DeleteTemplateAsync(int statementId)
    {
        await _templateClient.DeleteTemplateAsync(statementId);
    }

    public async Task UpdateStatementTemplateAsync(int templateId, string? title, string? filePath)
    {
        await _templateClient.UpdateTemplateAsync(templateId, new UpdateTemplateDto
        {
            Title = title,
            FilePath = filePath
        });
    }

    public async Task CreateStatementAsync(CreateTemplateModel templateModel)
    {
        var dto = _mapper.Map<CreateTemplateDto>(templateModel);
        
        await _templateClient.CreateTemplateAsync(dto);
    }

    public async Task DownloadTemplateAsync(int templateId, string savePath)
    {
        await using var stream = await _templateClient.DownloadTemplateAsync(templateId);
        await using var fileStream = File.Create(savePath);
        await stream.CopyToAsync(fileStream);
    }
    
    public async Task<IReadOnlyList<DynamicFieldInfoModel>> ExtractFieldsAsync(int templateId)
    {
        var result = await _templateClient.ExtractFieldsAsync(templateId);
        
        return _mapper.Map<IReadOnlyList<DynamicFieldInfoModel>>(result);
    }

    public async Task DeleteTemplatesAsync(List<int> templateIds)
    {
        await _templateClient.DeleteManyTemplateAsync(new DeleteManyTemplatesDto
        {
            TemplateIds = templateIds
        });
    }

    public async Task<IReadOnlyList<LookupItemModel>> GetTemplatesAsync()
    {
        var result = await _templateClient.GetTemplatesForFilterAsync();
        
        return result == null
            ? Array.Empty<LookupItemModel>()
            : _mapper.Map<IReadOnlyList<LookupItemModel>>(result);
    }

    public async Task CreateTaskAsync(int templateId, TemplateType templateType, Dictionary<string, object> data)
    {
        await _templateClient.CreateTask(new CreateTaskRequestDto
        {
            TemplateId = templateId,
            TemplateType = templateType,
            Data = data
        });
    }
}