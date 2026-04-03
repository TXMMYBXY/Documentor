using Documentor.Application.Api;
using Documentor.Application.Api.Admin;
using Documentor.Application.Api.Authorization;
using Documentor.Application.Api.Me;
using Documentor.Application.Api.Models;
using Documentor.Application.Services;
using Documentor.Core.Interfaces;
using Documentor.Core.Middleware;
using Documentor.Core.Services;
using Documentor.Core.Session;
using Documentor.Infrastructure.Api;
using Documentor.Infrastructure.Services;
using Documentor.Presentation.Factories;
using Documentor.Presentation.Navigation;
using Documentor.Presentation.ViewModels.Dialogs;
using Documentor.Presentation.ViewModels.Dialogs.User;
using Documentor.Presentation.ViewModels.Pages;
using Documentor.Presentation.ViewModels.Windows;
using Documentor.Presentation.Views.Dialogs;
using Documentor.Presentation.Views.Dialogs.Department;
using Documentor.Presentation.Views.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using INavigationService = Documentor.Presentation.Navigation.INavigationService;
using NavigationService = Documentor.Presentation.Navigation.NavigationService;

namespace Documentor.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DocumentFlowApi>(configuration.GetSection("DocumentFlowApi"));

        services.AddHttpClient<IGeneralClient, GeneralClient>();
        services.AddHttpClient<IAuthorizationClient, AuthorizationClient>();
        
        services.AddHttpClient<IPersonalAccountClient, PersonalAccountClient>()
            .AddHttpMessageHandler<AuthorizationHandler>();
        
        services.AddHttpClient<IAdminClient, AdminClient>()
            .AddHttpMessageHandler<AuthorizationHandler>();
        
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        services.AddSingleton<IUserSession, UserSession>();
        
        services.AddSingleton<IPageViewModelFactory, PageViewModelFactory>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IMenuFactory, MenuFactory>();

        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IApplicationNavigationService, ApplicationNavigationService>();
        services.AddTransient<IAppStartupService, AppStartupService>();
        services.AddTransient<IUserManagementService, UserManagementService>();
        services.AddTransient<IDepartmentManagementService, DepartmentManagementService>();

        services.AddTransient<IPersonalAccountService, PersonalAccountService>();
        services.AddTransient<IAuthorizationService, AuthorizationService>();
        services.AddSingleton<IDpapiService, DpapiService>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<IThemeService, ThemeService>();
        
        services.AddTransient<LoginWindowViewModel>();
        services.AddTransient<MainShellViewModel>();

        services.AddTransient<DashboardPageViewModel>();
        services.AddTransient<UsersPageViewModel>();
        services.AddTransient<DepartmentsPageViewModel>();
        services.AddTransient<ContractTemplatesPageViewModel>();
        services.AddTransient<StatementTemplatesPageViewModel>();
        services.AddTransient<TasksPageViewModel>();
        services.AddTransient<ProfilePageViewModel>();

        services.AddTransient<LoginWindow>();
        services.AddTransient<MainShellWindow>();
        
        services.AddTransient<UserFilterDialogWindow>();
        services.AddTransient<UserFilterDialogViewModel>();
        services.AddTransient<EditUserDialogWindow>();
        services.AddTransient<ResetPasswordDialogWindow>();
        
        services.AddTransient<AddDepartmentDialogWindow>();
        services.AddTransient<EditDepartmentDialogWindow>();
        services.AddTransient<DepartmentFilterDialogWindow>();
        
        services.AddTransient<AuthorizationHandler>();
        
        services.AddTransient<SettingsPageViewModel>();
        services.AddSingleton<IAppSettingsService, RegistryAppSettingsService>();
        
        return services;
    }
}