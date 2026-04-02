using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DocumentFlowing.Common;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;

namespace Documentor.Presentation.ViewModels.Dialogs
{
    public class AddUserViewModel : ViewModelBase
    {
        private readonly IUserManagementService _userManagementService;

        private string _email = string.Empty;
        private string _fullName = string.Empty;
        private string _password = string.Empty;
        private LookupItemModel? _selectedDepartment;
        private LookupItemModel? _selectedRole;
        private string _errorMessage = string.Empty;
        private bool _isBusy;

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                    (AddCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                if (SetProperty(ref _fullName, value))
                    (AddCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                    (AddCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public LookupItemModel? SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                if (SetProperty(ref _selectedDepartment, value))
                    (AddCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public LookupItemModel? SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (SetProperty(ref _selectedRole, value))
                    (AddCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
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

        public ObservableCollection<LookupItemModel> Departments { get; } = new();
        public ObservableCollection<LookupItemModel> Roles { get; } = new();

        public ICommand AddCommand { get; }
        public ICommand CancelCommand { get; }

        public Action<bool?>? CloseRequested { get; set; }

        public AddUserViewModel(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;

            AddCommand = new AsyncRelayCommand(AddAsync, _CanAddUser);
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(false));

            _ = LoadLookupsAsync();
        }

        private async Task LoadLookupsAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var departments = await _userManagementService.GetDepartmentsAsync();
                var roles = await _userManagementService.GetRolesAsync();

                Departments.Clear();
                foreach (var item in departments)
                    Departments.Add(item);

                Roles.Clear();
                foreach (var item in roles)
                    Roles.Add(item);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки справочников: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool _CanAddUser()
        {
            return !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(FullName) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   SelectedRole != null &&
                   SelectedDepartment != null;
        }

        private async Task AddAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                IsBusy = true;

                if (!_CanAddUser())
                {
                    ErrorMessage = "Заполните все обязательные поля.";
                    return;
                }

                var model = new CreateUserModel
                {
                    Email = Email,
                    FullName = FullName,
                    Password = Password,
                    RoleId = SelectedRole!.Id,
                    DepartmentId = SelectedDepartment!.Id
                };

                await _userManagementService.CreateUserAsync(model);
                CloseRequested?.Invoke(true);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка добавления пользователя: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}