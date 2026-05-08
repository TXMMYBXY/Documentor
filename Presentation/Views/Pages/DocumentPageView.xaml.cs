using System.ComponentModel;
using System.Windows.Controls;
using Documentor.Presentation.ViewModels.Pages;

namespace Documentor.Presentation.Views.Pages;

public partial class DocumentPageView : UserControl
{
    public DocumentPageView()
    {
        InitializeComponent();
    }
    
    private void DocumentsGrid_OnSorting(object sender, DataGridSortingEventArgs e)
    {
        e.Handled = true;

        if (DataContext is not DocumentPageViewModel vm)
            return;

        var column = e.Column;
        var direction = column.SortDirection != ListSortDirection.Ascending;

        column.SortDirection = direction
            ? ListSortDirection.Ascending
            : ListSortDirection.Descending;

        vm.ApplySorting(column.SortMemberPath, direction);
    }
}