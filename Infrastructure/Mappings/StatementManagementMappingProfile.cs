using AutoMapper;
using Documentor.Application.Api.Statement.Dtos;
using Documentor.Core.Models.Statement;

namespace Documentor.Infrastructure.Mappings;

public class StatementManagementMappingProfile : Profile
{
    public StatementManagementMappingProfile()
    {
        CreateMap<StatementFilterModel, StatementFilterDto>();

        CreateMap<GetTemplateDto, StatementListItemModel>()
            .ForMember(dest => dest.Owner,
                opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty));

        CreateMap<CreateStatementTemplateModel, CreateTemplateDto>();
    }
}