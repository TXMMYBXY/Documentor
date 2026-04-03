using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Application.Api.Admin.Dtos.User;

namespace Documentor.Core.Models.Department;

public class DepartmentListItemModel : ViewModelBase
{
    private int _id;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private IReadOnlyCollection<EmployeeDto> _employees = Array.Empty<EmployeeDto>();

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public IReadOnlyCollection<EmployeeDto> Employees
    {
        get => _employees;
        set => SetProperty(ref _employees, value);
    }

    public int EmployeesCount => Employees?.Count ?? 0;
}