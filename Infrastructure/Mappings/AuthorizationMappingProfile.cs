using AutoMapper;
using Documentor.Application.Api.Authorization.Dtos;
using Documentor.Application.Api.Authorization.Dtos.Responses;
using Documentor.Core.Enums;
using Documentor.Core.Models;
using Documentor.Core.Models.User;

namespace Documentor.Infrastructure.Mappings;

public class AuthorizationMappingProfile : Profile
{
    public AuthorizationMappingProfile()
    {
        CreateMap<AccessTokenResponseDto, LoginResponseDto>();

        CreateMap<AccessTokenResponseDto, AccessTokenDto>()
            .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.AccessToken))
            .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ExpiresAt))
            .ForMember(dest => dest.TokenType, opt => opt.MapFrom(src => src.TokenType));
        
        CreateMap<UserInfoDto, UserInfoModel>()
            .ForMember(
                dest => dest.Department,
                opt => opt.MapFrom(src => src.Department));
    }
}