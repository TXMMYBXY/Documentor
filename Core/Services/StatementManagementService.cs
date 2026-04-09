using System.IO;
using AutoMapper;
using Documentor.Application.Api.Statement;
using Documentor.Application.Api.Statement.Dtos;
using Documentor.Core.Enums;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.Statement;

namespace Documentor.Core.Services;

public class StatementManagementService : IStatementManagementService
{
    private readonly IStatementClient _statementClient;
    private readonly IMapper _mapper;

    public StatementManagementService(IStatementClient statementClient, IMapper mapper)
    {
        _statementClient = statementClient;
        _mapper = mapper;
    }

    public async Task<PagedResult<StatementListItemModel>> GetStatementsAsync(StatementFilterModel filter)
    {
        var filterDto = _mapper.Map<StatementFilterDto>(filter);
        var response = await _statementClient.GetStatementsAsync(filterDto);

        if (response == null)
        {
            return new PagedResult<StatementListItemModel>();
        }

        return new PagedResult<StatementListItemModel>
        {
            Items = _mapper.Map<IReadOnlyList<StatementListItemModel>>(response.Templates),
            TotalCount = response.TotalCount,
            PageSize = response.PageSize,
            CurrentPage = response.CurrentPage,
            TotalPages = response.TotalPages
        };
    }

    public async Task<bool> ChangeStatusAsync(int statementId)
    {
        return await _statementClient.ChangeTemplateStatusAsync(statementId);
    }

    public async Task DeleteStatementAsync(int statementId)
    {
        await _statementClient.DeleteTemplateAsync(statementId);
    }

    public async Task CreateStatementAsync(CreateStatementTemplateModel templateModel)
    {
        var dto = _mapper.Map<CreateTemplateDto>(templateModel);
        await _statementClient.CreateTemplateAsync(dto);
    }

    public async Task DownloadStatementTemplateAsync(int templateId, string savePath)
    {
        await using var stream = await _statementClient.DownloadTemplateAsync(templateId);
        await using var fileStream = File.Create(savePath);
        await stream.CopyToAsync(fileStream);
    }
    
    public async Task<IReadOnlyList<DynamicFieldInfoModel>> ExtractFieldsAsync(int templateId)
    {
        var result = await _statementClient.ExtractFieldsAsync(templateId);
        return _mapper.Map<IReadOnlyList<DynamicFieldInfoModel>>(result);
    }
    
    public async Task CreateTaskAsync(int templateId, Dictionary<string, object> data)
    {
        await _statementClient.CreateTask(new CreateTaskRequestDto
        {
            TemplateId = templateId,
            TemplateType = TemplateType.Statement,
            Data = data
        });
    }
}