using System.Windows.Input;
using DocumentFlowing.Common;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Interfaces;

namespace Documentor.Presentation.ViewModels.Dialogs;

public class ResetPasswordDialogViewModel : ViewModelBase
{
    private readonly IUserManagementService _userManagementService;
    private readonly int _userId;

    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public Action<bool?>? CloseRequested { get; set; }

    public ResetPasswordDialogViewModel(IUserManagementService userManagementService, int userId)
    {
        _userManagementService = userManagementService;
        _userId = userId;

        SaveCommand = new AsyncRelayCommand(SaveAsync);
        CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(false));
    }

    private async Task SaveAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "Введите пароль и подтверждение";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Пароли не совпадают";
                return;
            }

            await _userManagementService.ResetPasswordAsync(_userId, Password);
            CloseRequested?.Invoke(true);
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