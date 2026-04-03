using System.Windows.Input;
using DocumentFlowing.Common;
using Documentor.Core.Interfaces;
using Documentor.Core.Models.Department;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Dialogs.Department;

public class AddDepartmentViewModel : DialogViewModelBase
{
    private readonly IDepartmentManagementService _departmentManagementService;

    private string _title = string.Empty;
    private string _description = string.Empty;

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

    public ICommand AddCommand { get; }

    public AddDepartmentViewModel(IDepartmentManagementService departmentManagementService)
    {
        _departmentManagementService = departmentManagementService;
        AddCommand = new AsyncRelayCommand(AddAsync);
    }

    private async Task AddAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            await _departmentManagementService.CreateDepartmentAsync(new CreateDepartmentModel
            {
                Title = Title,
                Description = Description
            });

            RequestClose(true);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка создания отдела: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}