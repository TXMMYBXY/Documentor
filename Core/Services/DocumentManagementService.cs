using AutoMapper;
using Documentor.Application.Api.Document;
using Documentor.Application.Api.Document.Dtos;
using Documentor.Application.Api.Statement;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.Document;
using System.IO;

namespace Documentor.Core.Services;

public class DocumentManagementService : IDocumentManagementService
{
    private readonly IDocumentClient _documentClient;
    private readonly ITemplateClient _templateClient;
    private readonly IMapper _mapper;

    public DocumentManagementService(
        IDocumentClient documentClient,
        ITemplateClient templateClient,
        IMapper mapper)
    {
        _documentClient = documentClient;
        _templateClient = templateClient;
        _mapper = mapper;
    }
    
    public async Task<PagedResult<DocumentListItemModel>> GetAllDocumentsAsync(DocumentFilterModel filter)
    {
        var filterDto = _mapper.Map<DocumentFilterDto>(filter);

        var response = await _documentClient.GetDocumentsAsync(filterDto);

        ArgumentNullException.ThrowIfNull(response);

        return new PagedResult<DocumentListItemModel>
        {
            Items = _mapper.Map<IReadOnlyList<DocumentListItemModel>>(response.Documents),
            TotalCount = response.TotalCount,
            PageSize = response.PageSize,
            CurrentPage = response.CurrentPage
        };
    }

    public async Task DeleteDocumentAsync(int selectedDocumentId)
    {
        await _documentClient.DeleteDocumentByIdAsync(selectedDocumentId);
    }

    public async Task DownloadDocumentAsync(int selectedDocumentId, string dialogFileName)
    {
        await using var file = await _documentClient.DownloadDocumentAsync(selectedDocumentId);

        await using var outStream = File.Create(dialogFileName);
        await file.ContentStream.CopyToAsync(outStream);
    }
}