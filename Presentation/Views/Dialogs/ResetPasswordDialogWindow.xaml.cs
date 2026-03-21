using System.Windows;
using Documentor.Presentation.ViewModels.Dialogs;

namespace Documentor.Presentation.Views.Dialogs;

public partial class ResetPasswordDialogWindow : Window
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
}