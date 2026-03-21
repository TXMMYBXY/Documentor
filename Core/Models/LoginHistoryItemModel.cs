namespace Documentor.Core.Models;

public class LoginHistoryItemModel
{
    public DateTime? LoginTime { get; set; }

    public string LoginTimeFormatted =>
        LoginTime?.ToString("dd.MM.yyyy HH:mm:ss") ?? "-";
}