using System.Net.Http;
using System.Web;
using DocumentFlowing.Client.Models;
using Documentor.Application.Api.Admin;
using Documentor.Application.Api.Admin.Dtos;
using Documentor.Application.Api.Admin.Dtos.Department;
using Documentor.Application.Api.Admin.Dtos.User;
using Documentor.Application.Api.Models;
using Microsoft.Extensions.Options;

namespace Documentor.Infrastructure.Api;

public class AdminClient : GeneralClient, IAdminClient
{
    public AdminClient(HttpClient httpClient, IOptions<DocumentFlowApi> documentFlowApi)
        : base(httpClient, documentFlowApi)
    {
    }

    public async Task<GetUsersResponseDto?> GetUsersAsync(UserFilterDto filter)
    {
        var query = BuildUsersQuery(filter);
        return await GetResponseAsync<GetUsersResponseDto>($"users{query}");
    }

    public async Task CreateNewUserAsync(CreateUserDto createUserDto)
    {
        await PostResponseAsync<CreateUserDto, CreateUserDto>(createUserDto, "users");
    }

    public async Task<bool> ChangeStatusByIdAsync(int userId)
    {
        return await PatchResponseAsync<object, bool>(null, $"users/{userId}/change-status");
    }

    public async Task DeleteUserByIdAsync(int selectedUserId)
    {
        await DeleteResponseAsync<DeleteUserDto, object>(
            new DeleteUserDto { UserId = selectedUserId },
            "users");
    }

    public async Task ChangePasswordByIdAsync(int userId, ResetPasswordDto resetPasswordDto)
    {
        await PatchResponseAsync<ResetPasswordDto, object>(resetPasswordDto, $"users/{userId}/reset-password");
    }

    public async Task UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
    {
        await PatchResponseAsync<UpdateUserDto, object>(updateUserDto, $"users/{userId}/user-info");
    }

    public async Task<List<GetDepartmentDto>> GetAllDepartmentsAsync()
    {
        return await GetResponseAsync<List<GetDepartmentDto>>("department");
    }

    public async Task<List<GetRoleDto>> GetAllRolesAsync()
    {
        return await GetResponseAsync<List<GetRoleDto>>("role");
    }

    private static string BuildUsersQuery(UserFilterDto filter)
    {
        var parameters = new List<string>();

        if (!string.IsNullOrWhiteSpace(filter.FullName))
            parameters.Add($"FullName={Uri.EscapeDataString(filter.FullName)}");

        if (!string.IsNullOrWhiteSpace(filter.Email))
            parameters.Add($"Email={Uri.EscapeDataString(filter.Email)}");

        if (filter.DepartmentId.HasValue)
            parameters.Add($"DepartmentId={filter.DepartmentId.Value}");

        if (filter.RoleId.HasValue)
            parameters.Add($"RoleId={filter.RoleId.Value}");

        if (filter.PageSize.HasValue)
            parameters.Add($"PageSize={filter.PageSize.Value}");

        if (filter.PageNumber.HasValue)
            parameters.Add($"PageNumber={filter.PageNumber.Value}");

        if (parameters.Count == 0)
            return string.Empty;

        return "?" + string.Join("&", parameters);
    }
}