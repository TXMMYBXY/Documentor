using System.Windows;
using Documentor.Presentation.ViewModels.Dialogs;

namespace Documentor.Presentation.Views.Dialogs;

public partial class UserFilterDialogWindow : Window
{
    public UserFilterDialogWindow()
    {
        InitializeComponent();

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is UserFilterDialogViewModel vm)
        {
            vm.CloseRequested = result =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
}