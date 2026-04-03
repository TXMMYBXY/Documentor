using System.Windows.Input;
using DocumentFlowing.Common;
using Documentor.Core.Interfaces;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Dialogs.User;

public class ResetPasswordDialogViewModel : DialogViewModelBase
{
    private readonly IUserManagementService _userManagementService;
    private readonly int _userId;

    private string _newPassword = string.Empty;
    private string _confirmPassword = string.Empty;

    public string NewPassword
    {
        get => _newPassword;
        set => SetProperty(ref _newPassword, value);
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }

    public ICommand SaveCommand { get; }

    public ResetPasswordDialogViewModel(IUserManagementService userManagementService, int userId)
    {
        _userManagementService = userManagementService;
        _userId = userId;

        SaveCommand = new AsyncRelayCommand(SaveAsync);
    }

    private async Task SaveAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "Введите новый пароль и подтверждение.";
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "Пароли не совпадают.";
                return;
            }

            await _userManagementService.ResetPasswordAsync(_userId, NewPassword);

            RequestClose(true);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка смены пароля: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}