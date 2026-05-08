using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Documentor.Core.Models.Template;
using Documentor.Presentation.ViewModels.Pages;

namespace Documentor.Presentation.Views.Pages
{
    public partial class TemplatesPageView : UserControl
    {
        public TemplatesPageView()
        {
            InitializeComponent();
        }

        private void Root_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ClearStatementsGridSelection();
                e.Handled = true;
            }
        }

        private void Root_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (StatementsGrid == null)
                return;

            var dependencyObject = e.OriginalSource as DependencyObject;
            if (dependencyObject == null)
            {
                ClearStatementsGridSelection();
                return;
            }

            var clickedInsideDataGridRow = FindParent<DataGridRow>(dependencyObject) != null;
            var clickedInsideDataGridCell = FindParent<DataGridCell>(dependencyObject) != null;
            var clickedInsideScrollBar = FindParent<ScrollBar>(dependencyObject) != null;
            var clickedButton = FindParent<Button>(dependencyObject) != null;
            var clickedComboBox = FindParent<ComboBox>(dependencyObject) != null;

            if (!clickedInsideDataGridRow &&
                !clickedInsideDataGridCell &&
                !clickedInsideScrollBar &&
                !clickedButton &&
                !clickedComboBox)
            {
                ClearStatementsGridSelection();
            }
        }

        private void ClearStatementsGridSelection()
        {
            StatementsGrid.UnselectAll();
            StatementsGrid.SelectedItem = null;
            Keyboard.ClearFocus();
            Focus();
        }

        private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = child;

            while (parent != null)
            {
                if (parent is T correctlyTyped)
                    return correctlyTyped;

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
        
        private void StatementsGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is TemplatesPageViewModel vm)
            {
                var selected = StatementsGrid.SelectedItems
                    .Cast<TemplateListItemModel>()
                    .ToList();

                vm.UpdateSelection(selected);
            }
        }

        private void StatementsGrid_OnSorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;

            if (DataContext is not TemplatesPageViewModel vm)
                return;

            var column = e.Column;
            var direction = column.SortDirection != ListSortDirection.Ascending;

            column.SortDirection = direction
                ? ListSortDirection.Ascending
                : ListSortDirection.Descending;

            vm.ApplySorting(column.SortMemberPath, direction);
        }
    }
}