using AutoMapper;
using Documentor.Application.Api.Me.Dtos;
using Documentor.Core.Models;
using Documentor.Core.Models.Profile;

namespace Documentor.Infrastructure.Mappings;

public class PersonalAccountMappingProfile : Profile
{
    public PersonalAccountMappingProfile()
    {
        CreateMap<GetPersonDto, ProfileModel>()
            .ForMember(
                dest => dest.RoleTitle,
                opt => opt.MapFrom(src => src.Role != null ? src.Role.Title ?? string.Empty : string.Empty));

        CreateMap<GetLoginTimeDto, LoginHistoryItemModel>();

        CreateMap<ChangePasswordModel, ChangePasswordDto>();
    }
}