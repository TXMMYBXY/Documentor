using AutoMapper;
using Documentor.Application.Api.Admin;
using Documentor.Application.Api.Admin.Dtos.Department;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.Department;

namespace Documentor.Core.Services;

public class DepartmentManagementService : IDepartmentManagementService
{
    private readonly IAdminClient _adminClient;
    private readonly IMapper _mapper;

    public DepartmentManagementService(IAdminClient adminClient, IMapper mapper)
    {
        _adminClient = adminClient;
        _mapper = mapper;
    }
    
    public async Task<PagedResult<DepartmentListItemModel>> GetDepartmentAsync(DepartmentFilterModel filter)
    {
        var filterDto = _mapper.Map<DepartmentFilterDto>(filter);
        var response = await _adminClient.GetDepartmentsAsync(filterDto);

        if (response == null)
        {
            return new PagedResult<DepartmentListItemModel>();
        }

        return new PagedResult<DepartmentListItemModel>
        {
            Items = _mapper.Map<IReadOnlyList<DepartmentListItemModel>>(response.Departments),
            TotalCount = response.TotalCount,
            PageSize = response.PageSize,
            CurrentPage = response.CurrentPage,
            TotalPages = response.TotalPages
        };
    }

    public async Task DeleteDepartmentAsync(int departmentId)
    {
        throw new NotImplementedException();
    }

    public async Task CreateDepartmentAsync(CreateDepartmentModel model)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateDepartmentAsync(int departmentId, EditDepartmentModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<LookupItemModel>> GetUsersAsync()
    {
        throw new NotImplementedException();
    }
}