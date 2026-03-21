using AutoMapper;
using DocumentFlowing.Client.Authorization.Dtos;
using Documentor.Application.Api.Authorization.Dtos;
using Documentor.Core.Enums;
using Documentor.Core.Models;

namespace Documentor.Infrastructure.Mappings;

public class AuthorizationMappingProfile : Profile
{
    public AuthorizationMappingProfile()
    {
        CreateMap<AccessTokenResponseDto, LoginResponseDto>().ReverseMap();
        
        CreateMap<UserInfoDto, UserInfoModel>()
            .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => (UserRole)src.RoleId))
            .ForMember(
                dest => dest.RoleTitle,
                opt => opt.MapFrom(src => ((UserRole)src.RoleId).ToString()))
            .ForMember(
                dest => dest.Department,
                opt => opt.MapFrom(src => $"Отдел #{src.DepartmentId}"));
    }
}