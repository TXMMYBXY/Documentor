using System.Collections.ObjectModel;
using System.Windows.Input;
using DocumentFlowing.Common;
using Documentor.Application.Api.Admin.Dtos.User;
using Documentor.Common;
using Documentor.Core.Models.Department;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Dialogs.Department;

public class DepartmentEmployeesDialogViewModel : ViewModelBase
{
    private readonly Action _closeAction;

    public string Title { get; }
    public int EmployeesCount => Employees.Count;

    public ObservableCollection<EmployeeDto> Employees { get; } = new();

    public ICommand CloseCommand { get; }

    public DepartmentEmployeesDialogViewModel(DepartmentListItemModel department, Action closeAction)
    {
        _closeAction = closeAction;
        Title = $"Сотрудники отдела «{department.Title}»";

        foreach (var employee in department.Employees)
        {
            Employees.Add(employee);
        }

        CloseCommand = new RelayCommand(() => _closeAction());
    }
}