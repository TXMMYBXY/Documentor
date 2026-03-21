using AutoMapper;
using Documentor.Application.Api.Admin.Dtos;
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
    }
}