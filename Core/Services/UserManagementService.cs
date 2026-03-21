using AutoMapper;
using Documentor.Application.Api.Admin;
using Documentor.Application.Api.Admin.Dtos;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;

namespace Documentor.Core.Services;

public class UserManagementService : IUserManagementService
{
    private readonly IAdminClient _adminClient;
    private readonly IMapper _mapper;

    public UserManagementService(IAdminClient adminClient, IMapper mapper)
    {
        _adminClient = adminClient;
        _mapper = mapper;
    }

    public async Task<PagedResult<UserListItemModel>> GetUsersAsync(UserFilterModel filter)
    {
        var dto = _mapper.Map<UserFilterDto>(filter);
        var response = await _adminClient.GetUsersAsync(dto);

        if (response == null)
        {
            return new PagedResult<UserListItemModel>();
        }

        return new PagedResult<UserListItemModel>
        {
            Items = _mapper.Map<IReadOnlyList<UserListItemModel>>(response.Users),
            TotalCount = response.TotalCount,
            PageSize = response.PageSize,
            CurrentPage = response.CurrentPage,
            TotalPages = response.TotalPages
        };
    }

    public async Task<bool> ChangeStatusAsync(int userId)
    {
        return await _adminClient.ChangeStatusByIdAsync(userId);
    }

    public async Task DeleteUserAsync(int userId)
    {
        await _adminClient.DeleteUserByIdAsync(userId);
    }
}