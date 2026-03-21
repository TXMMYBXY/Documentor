using Documentor.Application.Api.Me.Dtos;

namespace Documentor.Application.Api.Me;

public interface IPersonalAccountClient
{
    Task<GetPersonDto?> GetPersonalInfoAsync(string uri);
    Task<IReadOnlyList<GetLoginTimeDto>?> GetLoginTimesAsync(string uri);
    Task ChangePasswordAsync(ChangePasswordDto request, string uri);
}