using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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
        : base(httpClient)
    {
    }

    public async Task<PagedUserDto?> GetUsersAsync(UserFilterDto filter)
    {
        var query = _BuildUsersQuery(filter);
        
        return await GetResponseAsync<PagedUserDto>($"user{query}");
    }

    public async Task CreateNewUserAsync(CreateUserDto createUserDto)
    {
        await PostResponseAsync<CreateUserDto, CreateUserDto>(createUserDto, "user");
    }

    public async Task<bool> ChangeStatusByIdAsync(int userId)
    {
        return await PatchResponseAsync<object, bool>(null, $"user/{userId}/change-status");
    }

    public async Task DeleteUserByIdAsync(int selectedUserId)
    {
        await DeleteResponseAsync<object>($"user/{selectedUserId}");
    }
    
    public async Task DeleteSelectedUsersAsync(List<int> usersIds)
    {
        await MultipleDeletionResponseAsync<List<int>, object>(usersIds, "user");
    }

    public async Task ChangePasswordByIdAsync(int userId, ResetPasswordDto resetPasswordDto)
    {
        await PatchResponseAsync<ResetPasswordDto, object>(resetPasswordDto, $"user/{userId}/reset-password");
    }

    public async Task UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
    {
        await PatchResponseAsync<UpdateUserDto, object>(updateUserDto, $"user/{userId}/user-info");
    }

    public async Task<PagedDepartmentDto> GetAllDepartmentsAsync()
    {
        return await GetResponseAsync<PagedDepartmentDto>($"department");
    }
    
    public async Task<PagedDepartmentDto> GetDepartmentsAsync(DepartmentFilterDto filter)
    {
        var query = _BuildDepartmentsQuery(filter);
        return await GetResponseAsync<PagedDepartmentDto>($"department{query}");
    }

    public async Task CreateNewDepartmentAsync(CreateDepartmentDto createDepartmentDto)
    {
        await PostResponseAsync<CreateDepartmentDto, CreateDepartmentDto>(createDepartmentDto, "department");
    }

    public async Task DeleteDepartmentByIdAsync(int departmentId)
    {
        await DeleteResponseAsync<object>($"department/{departmentId}");
    }

    public async Task UpdateDepartmentAsync(int departmentId, UpdateDepartmentDto updateDepartmentDto)
    {
        await PutResponseAsync<UpdateDepartmentDto, object>(updateDepartmentDto, $"department/{departmentId}");
    }

    private static string _BuildUsersQuery(UserFilterDto filter)
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
    
    private static string _BuildDepartmentsQuery(DepartmentFilterDto filter)
    {
        var parameters = new List<string>();

        if (!string.IsNullOrWhiteSpace(filter.Title))
            parameters.Add($"Title={Uri.EscapeDataString(filter.Title)}");

        if (filter.PageSize.HasValue)
            parameters.Add($"PageSize={filter.PageSize.Value}");

        if (filter.PageNumber.HasValue)
            parameters.Add($"PageNumber={filter.PageNumber.Value}");

        if (parameters.Count == 0)
            return string.Empty;

        return "?" + string.Join("&", parameters);
    }
}