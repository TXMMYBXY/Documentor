using Documentor.Application.Api.Admin.Dtos;

namespace Documentor.Application.Api.Admin;

public interface IAdminClient
{
    Task<GetUsersResponseDto?> GetUsersAsync(UserFilterDto filter);

    Task CreateNewUserAsync(CreateNewUserDto createNewUserDto);
    Task<bool> ChangeStatusByIdAsync(int userId);
    Task DeleteUserByIdAsync(int selectedUserId);
    Task ChangePasswordByIdAsync(int userId, ResetPasswordDto resetPasswordDto);
    Task UpdateUserAsync(int userId, UpdateUserDto updateUserDto);

    Task<List<GetDepartmentDto>> GetAllDepartmentsAsync();
    Task<List<GetRoleDto>> GetAllRolesAsync();
}