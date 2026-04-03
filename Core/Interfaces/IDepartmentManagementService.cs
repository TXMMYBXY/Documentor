using Documentor.Core.Models;
using Documentor.Core.Models.Department;

namespace Documentor.Core.Interfaces;

public interface IDepartmentManagementService
{
    Task<PagedResult<DepartmentListItemModel>> GetDepartmentAsync(DepartmentFilterModel filter);
    Task DeleteDepartmentAsync(int departmentId);
    Task CreateDepartmentAsync(CreateDepartmentModel model);
    Task UpdateDepartmentAsync(int departmentId, EditDepartmentModel model);
    
    Task<IReadOnlyList<LookupItemModel>> GetUsersAsync();
}