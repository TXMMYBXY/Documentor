using Documentor.Core.Models;
using Documentor.Core.Models.Profile;

namespace Documentor.Core.Interfaces;

public interface IPersonalAccountService
{
    Task<ProfileModel?> GetProfileAsync();
    Task<IReadOnlyList<LoginHistoryItemModel>> GetLoginHistoryAsync();
    Task ChangePasswordAsync(ChangePasswordModel model);
}