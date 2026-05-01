namespace Documentor.Core.Models.Profile;

public class LoginHistoryItemModel
{
    public DateTime? LoginTime { get; set; }

    public string LoginTimeFormatted =>
        LoginTime?.ToLocalTime().ToString("dd.MM.yyyy HH:mm") ?? "-";
}