using System.Windows.Input;
using DocumentFlowing.Common;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Interfaces;
using Documentor.Core.Models.Department;

namespace Documentor.Presentation.ViewModels.Dialogs.Department;

public class AddDepartmentViewModel : ViewModelBase
{
    private readonly IDepartmentManagementService _departmentManagementService;

    private string _title = string.Empty;
    private string _description = string.Empty;
    private string _errorMessage = string.Empty;
    private Action<bool>? _closeAction;

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

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public ICommand AddCommand { get; }
    public ICommand CancelCommand { get; }

    public AddDepartmentViewModel(IDepartmentManagementService departmentManagementService)
    {
        _departmentManagementService = departmentManagementService;

        AddCommand = new AsyncRelayCommand(AddAsync);
        CancelCommand = new RelayCommand(() => _closeAction?.Invoke(false));
    }

    public void SetCloseAction(Action<bool> closeAction)
    {
        _closeAction = closeAction;
    }

    private async Task AddAsync()
    {
        try
        {
            ErrorMessage = string.Empty;

            await _departmentManagementService.CreateDepartmentAsync(new CreateDepartmentModel
            {
                Title = Title,
                Description = Description
            });

            _closeAction?.Invoke(true);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка создания отдела: {ex.Message}";
        }
    }
}