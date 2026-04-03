using System.Windows;
using System.Windows.Controls;
using Documentor.Presentation.ViewModels.Dialogs.Common;
using Documentor.Presentation.ViewModels.Windows;
using Documentor.Presentation.Views.Dialogs.Common;
using MahApps.Metro.Controls;

namespace Documentor.Presentation.Views.Windows;

public partial class LoginWindow : MetroWindow
{
    public LoginWindow()
    {
        InitializeComponent();
        Loaded += LoginWindow_Loaded;
    }

    private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginWindowViewModel vm)
        {
            vm.SetOpenApiSettingsAction(OpenApiSettingsDialog);
        }
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginWindowViewModel vm && sender is PasswordBox passwordBox)
        {
            vm.Password = passwordBox.Password;
        }
    }

    private void OpenApiSettingsDialog()
    {
        if (DataContext is not LoginWindowViewModel loginVm)
            return;

        ApiSettingsDialogWindow? dialog = null;

        var vm = new ApiSettingsDialogViewModel(loginVm.GetApiEndpointProvider());
        vm.SetCloseAction(result => dialog!.DialogResult = result);

        dialog = new ApiSettingsDialogWindow
        {
            DataContext = vm,
            Owner = this
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            loginVm.RefreshApiUrl();
        }
    }
}