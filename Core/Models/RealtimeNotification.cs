using Documentor.Core.Enums;

namespace Documentor.Core.Models;

public sealed record RealtimeNotification
(
    NotificationKind Kind,
    NotificationSeverity Severity,
    string Title,
    string Message
);