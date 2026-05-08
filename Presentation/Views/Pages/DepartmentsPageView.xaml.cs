using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Documentor.Core.Models.Department;
using Documentor.Presentation.ViewModels.Pages;

namespace Documentor.Presentation.Views.Pages
{
    public partial class DepartmentsPageView : UserControl
    {
        public DepartmentsPageView()
        {
            InitializeComponent();
        }

        private void EmployeesButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            if (button.DataContext is not DepartmentListItemModel department)
                return;

            if (DataContext is not DepartmentsPageViewModel vm)
                return;

            vm.SelectedDepartment = department;
            vm.OpenEmployeesDialogFor(department);
        }

        private void Root_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ClearDepartmentsGridSelection();
                e.Handled = true;
            }
        }

        private void Root_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DepartmentsGrid == null)
                return;

            var dependencyObject = e.OriginalSource as DependencyObject;
            if (dependencyObject == null)
            {
                ClearDepartmentsGridSelection();
                return;
            }

            var clickedInsideDataGridRow = _FindParent<DataGridRow>(dependencyObject) != null;
            var clickedInsideDataGridCell = _FindParent<DataGridCell>(dependencyObject) != null;
            var clickedInsideScrollBar = _FindParent<ScrollBar>(dependencyObject) != null;
            var clickedButton = _FindParent<Button>(dependencyObject) != null;
            var clickedComboBox = _FindParent<ComboBox>(dependencyObject) != null;

            if (!clickedInsideDataGridRow &&
                !clickedInsideDataGridCell &&
                !clickedInsideScrollBar &&
                !clickedButton &&
                !clickedComboBox)
            {
                ClearDepartmentsGridSelection();
            }
        }

        private void ClearDepartmentsGridSelection()
        {
            DepartmentsGrid.UnselectAll();
            DepartmentsGrid.SelectedItem = null;

            Keyboard.ClearFocus();
            Focus();
        }

        private static T? _FindParent<T>(DependencyObject child) where T : DependencyObject
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
    }
}