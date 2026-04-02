using System.Windows;
using System.Windows.Controls;
using Documentor.Presentation.ViewModels.Windows;
using MahApps.Metro.Controls;

namespace Documentor.Presentation.Views.Windows;

public partial class LoginWindow : MetroWindow
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginWindowViewModel vm && sender is PasswordBox passwordBox)
        {
            vm.Password = passwordBox.Password;
        }
    }
}