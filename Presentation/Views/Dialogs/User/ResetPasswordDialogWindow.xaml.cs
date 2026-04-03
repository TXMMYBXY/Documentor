using System.Windows;
using System.Windows.Controls;
using Documentor.Presentation.ViewModels.Dialogs.User;
using MahApps.Metro.Controls;

namespace Documentor.Presentation.Views.Dialogs;

public partial class ResetPasswordDialogWindow : MetroWindow
{
    public ResetPasswordDialogWindow()
    {
        InitializeComponent();
    }

    private void NewPasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ResetPasswordDialogViewModel vm && sender is PasswordBox passwordBox)
        {
            vm.NewPassword = passwordBox.Password;
        }
    }

    private void ConfirmPasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ResetPasswordDialogViewModel vm && sender is PasswordBox passwordBox)
        {
            vm.ConfirmPassword = passwordBox.Password;
        }
    }
}