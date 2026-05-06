using AutoMapper;
using Documentor.Application.Api.Admin;
using Documentor.Application.Api.Admin.Dtos.User;
using Documentor.Application.Api.Models;
using Documentor.Core.Enums;
using Documentor.Core.Extensions;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.User;

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
        var filterDto = _mapper.Map<UserFilterDto>(filter);
        var response = await _adminClient.GetUsersAsync(filterDto);

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

    public async Task<IReadOnlyList<LookupItemModel>> GetDepartmentsAsync()
    {
        var result = await _adminClient.GetAllDepartmentsAsync();
        
        return result == null
            ? Array.Empty<LookupItemModel>()
            : _mapper.Map<IReadOnlyList<LookupItemModel>>(result.Departments);
    }

    public async Task<IReadOnlyList<LookupItemModel>> GetRolesAsync()
    {
        var list = new List<LookupItemModel>();
        
        foreach (var role in Enum.GetValues<Role>())
        {
            list.Add(new LookupItemModel
            {
                Id = (int)role,
                Title = role.GetDisplayName()
            });
        }
        
        return list;
    }

    public async Task CreateUserAsync(CreateUserModel model)
    {
        var dto = _mapper.Map<CreateUserDto>(model);

        await _adminClient.CreateNewUserAsync(dto);
    }

    public async Task UpdateUserAsync(int userId, EditUserModel model)
    {
        var dto = _mapper.Map<UpdateUserDto>(model);
        
        await _adminClient.UpdateUserAsync(userId, dto);
    }

    public async Task ResetPasswordAsync(int userId, string password)
    {
        await _adminClient.ChangePasswordByIdAsync(userId, new ResetPasswordDto
        {
            Password = password
        });
    }
}