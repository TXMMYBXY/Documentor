using System.Collections.ObjectModel;
using System.Windows.Input;
using DocumentFlowing.Common;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;

namespace Documentor.Presentation.ViewModels.Pages;

public class ProfilePageViewModel : ViewModelBase
{
    private readonly IPersonalAccountService _personalAccountService;

    private string _fullName = string.Empty;
    private string _email = string.Empty;
    private string _department = string.Empty;
    private string _role = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    private string _currentPassword = string.Empty;
    private string _newPassword = string.Empty;
    private string _confirmPassword = string.Empty;

    public string Title => "Профиль";

    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Department
    {
        get => _department;
        set => SetProperty(ref _department, value);
    }

    public string Role
    {
        get => _role;
        set => SetProperty(ref _role, value);
    }

    public string CurrentPassword
    {
        get => _currentPassword;
        set
        {
            if (SetProperty(ref _currentPassword, value))
            {
                _RaisePasswordCommand();
            }
        }
    }

    public string NewPassword
    {
        get => _newPassword;
        set
        {
            if (SetProperty(ref _newPassword, value))
            {
                _RaisePasswordCommand();
            }
        }
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            if (SetProperty(ref _confirmPassword, value))
            {
                _RaisePasswordCommand();
            }
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (SetProperty(ref _isLoading, value))
            {
                _RaisePasswordCommand();
            }
        }
    }

    public ObservableCollection<LoginHistoryItemModel> LoginHistory { get; } = new();

    public ICommand RefreshCommand { get; }
    public ICommand ChangePasswordCommand { get; }

    public ProfilePageViewModel(IPersonalAccountService personalAccountService)
    {
        _personalAccountService = personalAccountService;

        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        ChangePasswordCommand = new AsyncRelayCommand(ChangePasswordAsync, _CanChangePassword);
        

        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var profile = await _personalAccountService.GetProfileAsync();
            if (profile != null)
            {
                FullName = profile.FullName;
                Email = profile.Email;
                Department = profile.Department;
                Role = profile.RoleTitle;
            }

            var history = await _personalAccountService.GetLoginHistoryAsync();

            LoginHistory.Clear();
            foreach (var item in history)
            {
                LoginHistory.Add(item);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка загрузки профиля: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ChangePasswordAsync()
    {
        try
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "Заполните все поля пароля";
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "Новый пароль и подтверждение не совпадают";
                return;
            }

            IsLoading = true;

            await _personalAccountService.ChangePasswordAsync(new ChangePasswordModel
            {
                CurrentPassword = CurrentPassword,
                NewPassword = NewPassword,
                ConfirmPassword = ConfirmPassword
            });

            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка смены пароля: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private bool _CanChangePassword()
    {
        return !IsLoading
               && !string.IsNullOrWhiteSpace(CurrentPassword)
               && !string.IsNullOrWhiteSpace(NewPassword)
               && !string.IsNullOrWhiteSpace(ConfirmPassword);
    }
    
    private void _RaisePasswordCommand()
    {
        if (ChangePasswordCommand is AsyncRelayCommand cmd)
            cmd.RaiseCanExecuteChanged();
    }
}