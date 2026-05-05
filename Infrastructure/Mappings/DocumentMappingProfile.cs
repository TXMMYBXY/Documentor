using AutoMapper;
using Documentor.Application.Api.Document.Dtos;
using Documentor.Core.Models.Document;

namespace Documentor.Infrastructure.Mappings;

public class DocumentMappingProfile : Profile
{
    public DocumentMappingProfile()
    {
        CreateMap<DocumentFilterModel, DocumentFilterDto>();

        CreateMap<DocumentDto, DocumentListItemModel>();
    }
}