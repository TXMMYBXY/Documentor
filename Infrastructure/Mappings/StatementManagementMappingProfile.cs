using AutoMapper;
using Documentor.Application.Api.Statement.Dtos;
using Documentor.Core.Models;
using Documentor.Core.Models.Statement;

namespace Documentor.Infrastructure.Mappings;

public class StatementManagementMappingProfile : Profile
{
    public StatementManagementMappingProfile()
    {
        CreateMap<StatementFilterModel, StatementFilterDto>();

        CreateMap<GetTemplateDto, TemplateListItemModel>()
            .ForMember(dest => dest.Owner,
                opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.FullName : string.Empty));

        CreateMap<CreateStatementTemplateModel, CreateTemplateDto>();
        
        CreateMap<DynamicFieldInfoDto, DynamicFieldInfoModel>();
        
        CreateMap<GetTemplateDto, LookupItemModel>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));
    }
}