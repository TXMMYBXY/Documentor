using Documentor.Core.Models;

namespace Documentor.Core.Interfaces;

public interface IUserManagementService
{
    Task<PagedResult<UserListItemModel>> GetUsersAsync(UserFilterModel filter);
    Task<bool> ChangeStatusAsync(int userId);
    Task DeleteUserAsync(int userId);
}