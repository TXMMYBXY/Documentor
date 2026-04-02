using AutoMapper;
using Documentor.Application.Api.Admin.Dtos;
using Documentor.Application.Api.Admin.Dtos.Department;
using Documentor.Application.Api.Admin.Dtos.User;
using Documentor.Core.Models;

namespace Documentor.Infrastructure.Mappings;

public class UserManagementMappingProfile : Profile
{
    public UserManagementMappingProfile()
    {
        CreateMap<GetUserDto, UserListItemModel>()
            .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => src.RoleEntity != null ? src.RoleEntity.Title : string.Empty));

        CreateMap<UserFilterModel, UserFilterDto>();

        CreateMap<GetDepartmentDto, LookupItemModel>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));

        CreateMap<GetRoleDto, LookupItemModel>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));

        CreateMap<EditUserModel, UpdateUserDto>();

        CreateMap<CreateUserDto, CreateUserModel>().ReverseMap();
    }
}