using System.Net.Http;
using Documentor.Application.Api.Me;
using Documentor.Application.Api.Me.Dtos;
using Documentor.Application.Api.Models;
using Microsoft.Extensions.Options;

namespace Documentor.Infrastructure.Api;

public class PersonalAccountClient : GeneralClient, IPersonalAccountClient
{
    public PersonalAccountClient(HttpClient httpClient, IOptions<DocumentFlowApi> documentFlowApi)
        : base(httpClient, documentFlowApi)
    {
    }

    public async Task<GetPersonDto?> GetPersonalInfoAsync()
    {
        return await GetResponseAsync<GetPersonDto>("personal");
    }

    public async Task<IReadOnlyList<GetLoginTimeDto>?> GetLoginTimesAsync()
    {
        return await GetResponseAsync<IReadOnlyList<GetLoginTimeDto>>("personal/login-times");
    }

    public async Task ChangePasswordAsync(ChangePasswordDto request)
    {
        await PatchResponseAsync<ChangePasswordDto, object>(request, "personal/change-password");
    }
}