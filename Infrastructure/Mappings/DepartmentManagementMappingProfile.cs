using AutoMapper;
using Documentor.Application.Api.Admin.Dtos.Department;
using Documentor.Core.Models;
using Documentor.Core.Models.Department;

namespace Documentor.Infrastructure.Mappings;

public class DepartmentManagementMappingProfile : Profile
{
    public DepartmentManagementMappingProfile()
    {
        CreateMap<GetDepartmentDto, DepartmentListItemModel>();

        CreateMap<DepartmentFilterModel, DepartmentFilterDto>();

        CreateMap<GetDepartmentDto, LookupItemModel>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));
        
    }
}