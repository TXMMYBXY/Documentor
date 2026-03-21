using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Enums;

namespace Documentor.Presentation.Factories;

public interface IPageViewModelFactory
{
    ViewModelBase Create(PageKey pageKey);
}