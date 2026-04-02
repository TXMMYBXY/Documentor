using System.Windows;
using System.Windows.Controls;
using Documentor.Presentation.ViewModels.Pages;

namespace Documentor.Presentation.Views.Pages;

public partial class ProfilePageView : UserControl
{
    public ProfilePageView()
    {
        InitializeComponent();
    }
    
    private void CurrentPasswordInput_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ProfilePageViewModel vm)
            vm.CurrentPassword = CurrentPasswordInput.Password;
    }

    private void NewPasswordInput_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ProfilePageViewModel vm)
            vm.NewPassword = NewPasswordInput.Password;
    }

    private void ConfirmPasswordInput_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ProfilePageViewModel vm)
            vm.ConfirmPassword = ConfirmPasswordInput.Password;
    }
}