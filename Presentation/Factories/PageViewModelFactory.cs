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
            PageKey.StatementTemplates =>
                _CreateTemplatesPage(TemplateType.Statement, PageKey.StatementTemplates),

            PageKey.ContractTemplates =>
                _CreateTemplatesPage(TemplateType.Contract, PageKey.ContractTemplates),
            
            PageKey.ReportTemplates => 
                _CreateTemplatesPage(TemplateType.Report, PageKey.ReportTemplates),
            
            PageKey.ApprovalTemplates =>
                _CreateTemplatesPage(TemplateType.Approval, PageKey.ApprovalTemplates),

            PageKey.Users =>
                _serviceProvider.GetRequiredService<UsersPageViewModel>(),

            PageKey.Departments =>
                _serviceProvider.GetRequiredService<DepartmentsPageViewModel>(),

            PageKey.Profile =>
                _serviceProvider.GetRequiredService<ProfilePageViewModel>(),

            PageKey.Settings =>
                _serviceProvider.GetRequiredService<SettingsPageViewModel>(),

            PageKey.Archive =>
                _serviceProvider.GetRequiredService<DocumentPageViewModel>(),

            _ => throw new NotSupportedException($"Page {pageKey} is not supported")
        };
    }
    
    private TemplatesPageViewModel _CreateTemplatesPage(TemplateType type, PageKey pageKey)
    {
        return ActivatorUtilities.CreateInstance<TemplatesPageViewModel>(_serviceProvider, type, pageKey);
    }
}