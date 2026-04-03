namespace Documentor.Core.Models.Department;

public class EditDepartmentModel
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public List<int>? EmployeesIds { get; set; }
}