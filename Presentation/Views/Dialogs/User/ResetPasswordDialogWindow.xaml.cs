using System.Windows;
using Documentor.Presentation.ViewModels.Dialogs;
using Documentor.Presentation.ViewModels.Dialogs.User;
using MahApps.Metro.Controls;

namespace Documentor.Presentation.Views.Dialogs;

public partial class ResetPasswordDialogWindow : MetroWindow
{
    public ResetPasswordDialogWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ResetPasswordDialogViewModel vm)
        {
            vm.CloseRequested = result =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
    
    private void PasswordInput_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ResetPasswordDialogViewModel vm)
            vm.Password = PasswordInput.Password;
    }

    private void ConfirmPasswordInput_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ResetPasswordDialogViewModel vm)
            vm.ConfirmPassword = ConfirmPasswordInput.Password;
    }
}