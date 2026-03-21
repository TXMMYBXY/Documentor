using Documentor.Core.Models;

namespace Documentor.Core.Interfaces;

public interface IPersonalAccountService
{
    Task<ProfileModel?> GetProfileAsync();
    Task<IReadOnlyList<LoginHistoryItemModel>> GetLoginHistoryAsync();
    Task ChangePasswordAsync(ChangePasswordModel model);
}