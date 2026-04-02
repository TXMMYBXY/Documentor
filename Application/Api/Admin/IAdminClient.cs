using Documentor.Application.Api.Admin.Dtos;
using Documentor.Application.Api.Admin.Dtos.Department;
using Documentor.Application.Api.Admin.Dtos.User;

namespace Documentor.Application.Api.Admin;

public interface IAdminClient
{
    Task<GetUsersResponseDto?> GetUsersAsync(UserFilterDto filter);
    Task CreateNewUserAsync(CreateUserDto createUserDto);
    Task<bool> ChangeStatusByIdAsync(int userId);
    Task DeleteUserByIdAsync(int selectedUserId);
    Task ChangePasswordByIdAsync(int userId, ResetPasswordDto resetPasswordDto);
    Task UpdateUserAsync(int userId, UpdateUserDto updateUserDto);

    Task<List<GetDepartmentDto>> GetAllDepartmentsAsync();
    Task CreateNewDepartmentAsync(CreateDepartmentDto createDepartmentDto);
    Task DeleteDepartmentByIdAsync(int departmentId);
    Task UpdateDepartmentAsync(int departmentId, UpdateDepartmentDto updateDepartmentDto);
    
    Task<List<GetRoleDto>> GetAllRolesAsync();
}