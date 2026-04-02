using System.Windows;
using Documentor.Presentation.ViewModels.Dialogs;
using MahApps.Metro.Controls;

namespace Documentor.Presentation.Views.Dialogs
{
    public partial class AddUserDialogWindow : MetroWindow
    {
        public AddUserDialogWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddUserViewModel vm)
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
            if (DataContext is AddUserViewModel vm)
                vm.Password = PasswordInput.Password;
        }
    }
}