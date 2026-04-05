using Documentor.Core.Models;
using Documentor.Core.Models.User;

namespace Documentor.Core.Interfaces;

public interface IUserManagementService
{
    Task<PagedResult<UserListItemModel>> GetUsersAsync(UserFilterModel filter);
    Task<bool> ChangeStatusAsync(int userId);
    Task DeleteUserAsync(int userId);
    Task CreateUserAsync(CreateUserModel model);
    Task UpdateUserAsync(int userId, EditUserModel model);
    Task ResetPasswordAsync(int userId, string password);
    Task<IReadOnlyList<LookupItemModel>> GetDepartmentsAsync();
    Task<IReadOnlyList<LookupItemModel>> GetRolesAsync();

}