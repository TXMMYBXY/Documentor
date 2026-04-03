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
        CreateMap<GetUserDto, UserListItemModel>()
            .AfterMap((src, dest) =>
            {
                switch (src.RoleEntity.Title)
                {
                    case "Admin":
                        dest.Role = "Администратор";
                        break;
                    case "Boss":
                        dest.Role = "Начальник закупок";
                        break;
                    case "Purchaser":
                        dest.Role = "Сотрудник закупок";
                        break;
                    case "Employee":
                        dest.Role = "Сотрудник";
                        break;
                
                    default:
                        dest.Role = "Неизвестная роль";
                        break;
                }
                
            });

        CreateMap<UserFilterModel, UserFilterDto>();

        CreateMap<GetDepartmentDto, LookupItemModel>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));

        CreateMap<GetRoleDto, LookupItemModel>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));

        CreateMap<EditUserModel, UpdateUserDto>();

        CreateMap<CreateUserDto, CreateUserModel>().ReverseMap();
    }
}