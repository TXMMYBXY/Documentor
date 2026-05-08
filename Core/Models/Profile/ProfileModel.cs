using Documentor.Core.Enums;

namespace Documentor.Core.Models.Profile;

public class ProfileModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public Role Role { get; set; }
}