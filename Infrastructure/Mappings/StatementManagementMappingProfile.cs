using AutoMapper;
using Documentor.Application.Api.Statement.Dtos;
using Documentor.Core.Models;
using Documentor.Core.Models.Template;

namespace Documentor.Infrastructure.Mappings;

public class StatementManagementMappingProfile : Profile
{
    public StatementManagementMappingProfile()
    {
        CreateMap<TemplateFilterModel, TemplateFilterDto>();

        CreateMap<GetTemplateDto, TemplateListItemModel>()
            .ForMember(dest => dest.CreatedBy,
                opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.FullName : string.Empty));

        CreateMap<CreateTemplateModel, CreateTemplateDto>();
        
        CreateMap<DynamicFieldInfoDto, DynamicFieldInfoModel>();
        
        CreateMap<GetTemplateDto, LookupItemModel>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));
    }
}