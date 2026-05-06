using System;
using System.Net.Http;
using Documentor.Application.Api;
using Documentor.Application.Api.Admin;
using Documentor.Application.Api.Authorization;
using Documentor.Application.Api.Document;
using Documentor.Application.Api.Me;
using Documentor.Application.Api.Models;
using Documentor.Application.Api.Statement;
using Documentor.Application.Services;
using Documentor.Core.Interfaces;
using Documentor.Core.Middleware;
using Documentor.Core.Services;
using Documentor.Core.Session;
using Documentor.Infrastructure.Api;
using Documentor.Infrastructure.Services;
using Documentor.Presentation.Factories;
using Documentor.Presentation.Navigation;
using Documentor.Presentation.Services;
using Documentor.Presentation.ViewModels.Dialogs.Common;
using Documentor.Presentation.ViewModels.Dialogs.Document;
using Documentor.Presentation.ViewModels.Dialogs.User;
using Documentor.Presentation.ViewModels.Pages;
using Documentor.Presentation.ViewModels.Windows;
using Documentor.Presentation.Views.Dialogs.Common;
using Documentor.Presentation.Views.Dialogs.Department;
using Documentor.Presentation.Views.Dialogs.Document;
using Documentor.Presentation.Views.Dialogs.User;
using Documentor.Presentation.Views.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using INavigationService = Documentor.Presentation.Navigation.INavigationService;
using NavigationService = Documentor.Presentation.Navigation.NavigationService;

namespace Documentor.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DocumentFlowApi>(configuration.GetSection("DocumentFlowApi"));
        
        services.AddHttpClient("DocumentFlowApi", _ConfigureApiClient);
        
        services.AddHttpClient<IGeneralClient, GeneralClient>(_ConfigureApiClient);
        services.AddHttpClient<IAuthorizationClient, AuthorizationClient>(_ConfigureApiClient);
        
        services.AddHttpClient<IPersonalAccountClient, PersonalAccountClient>(_ConfigureApiClient)
            .AddHttpMessageHandler<AuthorizationHandler>();
        
        services.AddHttpClient<IAdminClient, AdminClient>(_ConfigureApiClient)
            .AddHttpMessageHandler<AuthorizationHandler>();
        
        services.AddHttpClient<ITemplateClient, TemplateClient>(_ConfigureApiClient)
            .AddHttpMessageHandler<AuthorizationHandler>();
        
        services.AddHttpClient<IDocumentClient, DocumentClient>(_ConfigureApiClient)
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
        services.AddTransient<ITemplateManagementService, TemplateManagementService>();
        services.AddTransient<IDocumentManagementService, DocumentManagementService>();

        services.AddTransient<IPersonalAccountService, PersonalAccountService>();
        services.AddTransient<IAuthorizationService, AuthorizationService>();
        services.AddSingleton<IDpapiService, DpapiService>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<IThemeService, ThemeService>();
        
        services.AddTransient<LoginWindowViewModel>();
        services.AddTransient<MainShellViewModel>();

        services.AddTransient<UsersPageViewModel>();
        services.AddTransient<DepartmentsPageViewModel>();
        services.AddTransient<ContractTemplatesPageViewModel>();
        services.AddTransient<TemplatesPageViewModel>();
        services.AddTransient<ProfilePageViewModel>();
        services.AddTransient<DocumentPageViewModel>();

        services.AddTransient<LoginWindow>();
        services.AddTransient<MainShellWindow>();
        
        services.AddTransient<UserFilterDialogWindow>();
        services.AddTransient<UserFilterDialogViewModel>();
        services.AddTransient<EditUserDialogWindow>();
        services.AddTransient<ResetPasswordDialogWindow>();
        services.AddTransient<DocumentFilterDialogViewModel>();
        
        services.AddTransient<AddDepartmentDialogWindow>();
        services.AddTransient<EditDepartmentDialogWindow>();
        services.AddTransient<DepartmentFilterDialogWindow>();
        services.AddTransient<DocumentFilterDialogWindow>();
        
        services.AddTransient<AuthorizationHandler>();
        
        services.AddTransient<SettingsPageViewModel>();
        services.AddSingleton<IAppSettingsService, RegistryAppSettingsService>();
        
        services.AddSingleton<IApiEndpointProvider, ApiEndpointProvider>();

        services.AddTransient<ApiSettingsDialogViewModel>();
        services.AddTransient<ApiSettingsDialogWindow>();

        services.AddTransient<ConfirmationDialogViewModel>();
        services.AddTransient<ConfirmationDialogWindow>();
        
        services.AddSingleton<INotificationRealtimeService, NotificationRealtimeService>();
        services.AddSingleton<IDocumentRealtimeService, DocumentRealtimeService>();
        
        services.AddSingleton<INotificationCoordinator, NotificationCoordinator>();
        services.AddSingleton<ToastNotificationService>();

        services.AddSingleton<IToastNotificationService>(sp => sp.GetRequiredService<ToastNotificationService>());
        services.AddSingleton<IInAppToastSource>(sp => sp.GetRequiredService<ToastNotificationService>());

        return services;
    }
    
    private static void _ConfigureApiClient(IServiceProvider sp, HttpClient client)
    {
        var api = sp.GetRequiredService<IOptions<DocumentFlowApi>>().Value;
        client.BaseAddress = new Uri(api.Domain);
    }
}