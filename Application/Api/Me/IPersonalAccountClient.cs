using Documentor.Application.Api.Me.Dtos;

namespace Documentor.Application.Api.Me;

public interface IPersonalAccountClient
{
    Task<GetPersonDto?> GetPersonalInfoAsync();
    Task<IReadOnlyList<GetLoginTimeDto>?> GetLoginTimesAsync();
    Task ChangePasswordAsync(ChangePasswordDto request);
}