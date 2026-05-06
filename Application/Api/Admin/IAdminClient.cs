using System.Collections.Generic;
using System.Threading.Tasks;
using Documentor.Application.Api.Admin.Dtos;
using Documentor.Application.Api.Admin.Dtos.Department;
using Documentor.Application.Api.Admin.Dtos.User;

namespace Documentor.Application.Api.Admin;

public interface IAdminClient
{
    Task<PagedUserDto?> GetUsersAsync(UserFilterDto filter);
    Task CreateNewUserAsync(CreateUserDto createUserDto);
    Task<bool> ChangeStatusByIdAsync(int userId);
    Task DeleteUserByIdAsync(int selectedUserId);
    Task ChangePasswordByIdAsync(int userId, ResetPasswordDto resetPasswordDto);
    Task UpdateUserAsync(int userId, UpdateUserDto updateUserDto);
    Task<PagedDepartmentDto> GetAllDepartmentsAsync();

    Task<PagedDepartmentDto> GetDepartmentsAsync(DepartmentFilterDto filter);
    Task CreateNewDepartmentAsync(CreateDepartmentDto createDepartmentDto);
    Task DeleteDepartmentByIdAsync(int departmentId);
    Task UpdateDepartmentAsync(int departmentId, UpdateDepartmentDto updateDepartmentDto);
}