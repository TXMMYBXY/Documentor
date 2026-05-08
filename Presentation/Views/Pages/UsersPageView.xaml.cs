using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Documentor.Presentation.Views.Pages
{
    public partial class UsersPageView : UserControl
    {
        public UsersPageView()
        {
            InitializeComponent();
        }

        private void Root_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ClearUsersGridSelection();
                e.Handled = true;
            }
        }

        private void Root_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (UsersGrid == null)
                return;

            var dependencyObject = e.OriginalSource as DependencyObject;
            if (dependencyObject == null)
            {
                ClearUsersGridSelection();
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
                ClearUsersGridSelection();
            }
        }

        private void ClearUsersGridSelection()
        {
            UsersGrid.UnselectAll();
            UsersGrid.SelectedItem = null;
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
    }
}