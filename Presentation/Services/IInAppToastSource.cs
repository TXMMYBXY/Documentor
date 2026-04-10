using System.Collections.ObjectModel;
using Documentor.Presentation.Services.Toasts;

namespace Documentor.Presentation.Services;

public interface IInAppToastSource
{
    ReadOnlyObservableCollection<ToastItemViewModel> Notifications { get; }
}