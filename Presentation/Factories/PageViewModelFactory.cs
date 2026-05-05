using Documentor.Core.Enums;
using Documentor.Presentation.ViewModels.Base;
using Documentor.Presentation.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Documentor.Presentation.Factories;

public class PageViewModelFactory : IPageViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PageViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ViewModelBase Create(PageKey pageKey)
    {
        return pageKey switch
        {
            PageKey.Dashboard => _serviceProvider.GetRequiredService<DashboardPageViewModel>(),
            PageKey.Users => _serviceProvider.GetRequiredService<UsersPageViewModel>(),
            PageKey.Departments => _serviceProvider.GetRequiredService<DepartmentsPageViewModel>(),
            PageKey.ContractTemplates => _serviceProvider.GetRequiredService<ContractTemplatesPageViewModel>(),
            PageKey.StatementTemplates => _serviceProvider.GetRequiredService<TemplatesPageViewModel>(),
            PageKey.Profile => _serviceProvider.GetRequiredService<ProfilePageViewModel>(),
            PageKey.Settings => _serviceProvider.GetRequiredService<SettingsPageViewModel>(),
            PageKey.Archive => _serviceProvider.GetRequiredService<DocumentPageViewModel>(),
            _ => throw new NotSupportedException($"Page {pageKey} is not supported")
        };
    }
}