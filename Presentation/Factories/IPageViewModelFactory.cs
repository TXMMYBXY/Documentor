using Documentor.Core.Enums;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.Factories;

public interface IPageViewModelFactory
{
    ViewModelBase Create(PageKey pageKey);
}