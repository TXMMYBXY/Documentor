using System.Diagnostics;
using AutoMapper;
using Documentor.Application.Api.Admin.Dtos;
using Documentor.Application.Api.Admin.Dtos.Department;
using Documentor.Application.Api.Admin.Dtos.User;
using Documentor.Core.Models;
using Documentor.Core.Models.User;

namespace Documentor.Infrastructure.Mappings;

public class UserManagementMappingProfile : Profile
{
    public UserManagementMappingProfile()
    {
        CreateMap<UserDto, UserListItemModel>()
            .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department.Title));
        CreateMap<UserFilterModel, UserFilterDto>();

        CreateMap<GetDepartmentDto, LookupItemModel>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));

        CreateMap<EditUserModel, UpdateUserDto>();

        CreateMap<CreateUserDto, CreateUserModel>().ReverseMap();
    }
}