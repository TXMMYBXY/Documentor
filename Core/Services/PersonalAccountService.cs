using AutoMapper;
using Documentor.Application.Api.Me;
using Documentor.Application.Api.Me.Dtos;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;

namespace Documentor.Core.Services;

public class PersonalAccountService : IPersonalAccountService
{
    private readonly IPersonalAccountClient _personalAccountClient;
    private readonly IMapper _mapper;

    public PersonalAccountService(
        IPersonalAccountClient personalAccountClient,
        IMapper mapper)
    {
        _personalAccountClient = personalAccountClient;
        _mapper = mapper;
    }

    public async Task<ProfileModel?> GetProfileAsync()
    {
        var dto = await _personalAccountClient.GetPersonalInfoAsync("personal");

        if (dto == null)
        {
            return null;
        }

        return _mapper.Map<ProfileModel>(dto);
    }

    public async Task<IReadOnlyList<LoginHistoryItemModel>> GetLoginHistoryAsync()
    {
        var dto = await _personalAccountClient.GetLoginTimesAsync("personal/login-times");
        if (dto == null)
            return Array.Empty<LoginHistoryItemModel>();

        return _mapper.Map<IReadOnlyList<LoginHistoryItemModel>>(dto);
    }

    public async Task ChangePasswordAsync(ChangePasswordModel model)
    {
        var dto = _mapper.Map<ChangePasswordDto>(model);
        
        await _personalAccountClient.ChangePasswordAsync(dto, "personal/change-password");
    }
}