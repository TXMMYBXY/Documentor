using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DocumentFlowing.Common;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.Statement;
using Documentor.Presentation.ViewModels.Base;
using Documentor.Presentation.ViewModels.Dialogs.Common;
using Documentor.Presentation.ViewModels.Dialogs.Statement;
using Documentor.Presentation.Views.Dialogs.Common;
using Documentor.Presentation.Views.Dialogs.Statement;
using Microsoft.Win32;

namespace Documentor.Presentation.ViewModels.Pages;

public class StatementTemplatesPageViewModel : PagedListPageViewModel<StatementListItemModel, StatementFilterModel>
{
    private readonly IStatementManagementService _statementManagementService;
    private readonly IAppSettingsService _appSettingsService;

    public static string Title => "Шаблоны заявлений";

    public ObservableCollection<StatementListItemModel> Statements => Items;

    public StatementListItemModel? SelectedStatement
    {
        get => SelectedItem;
        set => SelectedItem = value;
    }

    public override string ActiveFilterSummary => _BuildFilterSummary();

    public ICommand OpenFilterCommand { get; }
    public ICommand AddStatementCommand { get; }
    public ICommand ChangeStatusCommand { get; }
    public ICommand DeleteStatementCommand { get; }
    public ICommand FillStatementCommand { get; }
    public ICommand DownloadStatementCommand { get; }
    public ICommand EditStatementCommand { get; }
    

    public StatementTemplatesPageViewModel(
        IStatementManagementService statementManagementService,
        IAppSettingsService appSettingsService)
    {
        _statementManagementService = statementManagementService;
        _appSettingsService = appSettingsService;

        InitializePageSize(_appSettingsService.GetPageSize());

        CurrentFilter = new StatementFilterModel
        {
            PageNumber = 1,
            PageSize = PageSize
        };

        OpenFilterCommand = new RelayCommand(_OpenFilter);
        AddStatementCommand = new RelayCommand(_AddStatement);
        ChangeStatusCommand = new AsyncRelayCommand(_ChangeStatusAsync, () => SelectedStatement != null);
        DeleteStatementCommand = new AsyncRelayCommand(_DeleteStatementAsync, () => SelectedStatement != null);
        FillStatementCommand = new RelayCommand(_FillStatement, () => SelectedStatement != null && SelectedStatement.IsActive);
        DownloadStatementCommand = new AsyncRelayCommand(_DownloadStatementAsync, () => SelectedStatement != null);
        EditStatementCommand = new RelayCommand(_EditStatement, () => SelectedStatement != null);

        _ = LoadAsync();
    }

    protected override void ApplyPagingToFilter()
    {
        CurrentFilter.PageNumber = CurrentPage;
        CurrentFilter.PageSize = PageSize;
    }

    protected override async Task<PagedResult<StatementListItemModel>> LoadPageAsync()
    {
        return await _statementManagementService.GetStatementsAsync(CurrentFilter);
    }

    protected override async Task ClearFilterAsync()
    {
        var pageSize = _appSettingsService.GetPageSize();

        CurrentFilter = new StatementFilterModel
        {
            PageNumber = 1,
            PageSize = pageSize
        };

        CurrentPage = 1;
        PageSize = pageSize;

        await LoadAsync();
    }

    protected override void RaiseSelectionCommands()
    {
        if (ChangeStatusCommand is AsyncRelayCommand changeStatus)
            changeStatus.RaiseCanExecuteChanged();

        if (DeleteStatementCommand is AsyncRelayCommand delete)
            delete.RaiseCanExecuteChanged();

        if (FillStatementCommand is RelayCommand fill)
            fill.RaiseCanExecuteChanged();

        if (DownloadStatementCommand is AsyncRelayCommand download)
            download.RaiseCanExecuteChanged();
        
        if (EditStatementCommand is RelayCommand edit)
            edit.RaiseCanExecuteChanged();
    }

    protected override string BuildLoadErrorMessage(Exception ex)
    {
        return $"Ошибка загрузки шаблонов заявлений: {ex.Message}";
    }

    private void _OpenFilter()
    {
        StatementFilterDialogWindow? dialog = null;

        var vm = new StatementFilterDialogViewModel(new StatementFilterModel
        {
            Title = CurrentFilter.Title,
            CreatedBy = CurrentFilter.CreatedBy,
            CreatedAtEarlier = CurrentFilter.CreatedAtEarlier,
            CreatedAtLater = CurrentFilter.CreatedAtLater,
            PageSize = CurrentFilter.PageSize
        });

        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new StatementFilterDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            CurrentFilter = vm.ResultFilter;
            CurrentPage = 1;
            PageSize = CurrentFilter.PageSize ?? _appSettingsService.GetPageSize();
            _ = LoadAsync();
        }
    }

    private void _AddStatement()
    {
        AddStatementTemplateDialogWindow? dialog = null;

        var vm = new AddStatementTemplateDialogViewModel(_statementManagementService);
        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new AddStatementTemplateDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            _ = LoadAsync();
        }
    }
    
    private void _EditStatement()
    {
        if (SelectedStatement == null)
            return;

        EditStatementTemplateDialogWindow? dialog = null;

        var vm = new EditStatementTemplateDialogViewModel(
            _statementManagementService,
            SelectedStatement.Id,
            SelectedStatement);

        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new EditStatementTemplateDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        var result = dialog.ShowDialog();
        if (result == true)
            _ = LoadAsync();
    }

    private async Task _ChangeStatusAsync()
    {
        if (SelectedStatement == null)
            return;

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var newStatus = await _statementManagementService.ChangeStatusAsync(SelectedStatement.Id);
            SelectedStatement.IsActive = newStatus;

            RaiseSelectionCommands();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка изменения статуса: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task _DeleteStatementAsync()
    {
        if (SelectedStatement == null)
            return;

        ConfirmationDialogWindow? dialog = null;

        var vm = new ConfirmationDialogViewModel(
            "Удаление шаблона заявления",
            $"Удалить шаблон \"{SelectedStatement.Title}\"?",
            result => dialog!.DialogResult = result,
            "Удалить",
            "Отмена");

        dialog = new ConfirmationDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        var confirm = dialog.ShowDialog();
        if (confirm != true)
            return;

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            await _statementManagementService.DeleteStatementAsync(SelectedStatement.Id);

            if (Statements.Count == 1 && CurrentPage > 1)
            {
                CurrentPage--;
            }

            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка удаления шаблона: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void _FillStatement()
    {
        if (SelectedStatement == null)
            return;

        FillStatementTemplateDialogWindow? dialog = null;

        var vm = new FillStatementTemplateDialogViewModel(
            _statementManagementService,
            SelectedStatement.Id,
            SelectedStatement.Title);

        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new FillStatementTemplateDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        dialog.ShowDialog();
    }

    private async Task _DownloadStatementAsync()
    {
        if (SelectedStatement == null)
            return;

        try
        {
            ErrorMessage = string.Empty;

            var dialog = new SaveFileDialog
            {
                Filter = "Word Document (*.docx)|*.docx",
                FileName = $"{SelectedStatement.Title}.docx"
            };

            if (dialog.ShowDialog() != true)
                return;

            await _statementManagementService.DownloadStatementTemplateAsync(SelectedStatement.Id, dialog.FileName);

            MessageBox.Show("Шаблон успешно сохранён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка скачивания шаблона: {ex.Message}";
        }
    }

    private string _BuildFilterSummary()
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(CurrentFilter.Title))
            parts.Add($"Название: {CurrentFilter.Title}");

        if (CurrentFilter.CreatedAtEarlier.HasValue)
            parts.Add($"Создан до: {CurrentFilter.CreatedAtEarlier:dd.MM.yyyy}");

        if (CurrentFilter.CreatedAtLater.HasValue)
            parts.Add($"Создан после: {CurrentFilter.CreatedAtLater:dd.MM.yyyy}");

        return parts.Count == 0
            ? "Фильтр не применён"
            : string.Join(" | ", parts);
    }
}