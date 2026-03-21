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

    public async Task<GetPersonDto?> GetPersonalInfoAsync(string uri)
    {
        return await GetResponseAsync<GetPersonDto>(uri);
    }

    public async Task<IReadOnlyList<GetLoginTimeDto>?> GetLoginTimesAsync(string uri)
    {
        return await GetResponseAsync<IReadOnlyList<GetLoginTimeDto>>(uri);
    }

    public async Task ChangePasswordAsync(ChangePasswordDto request, string uri)
    {
        await PatchResponseAsync<ChangePasswordDto, object>(request, uri);
    }
}