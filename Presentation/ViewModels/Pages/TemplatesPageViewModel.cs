using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Documentor.Common;
using Documentor.Core.Enums;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.Template;
using Documentor.Presentation.ViewModels.Base;
using Documentor.Presentation.ViewModels.Dialogs.Common;
using Documentor.Presentation.ViewModels.Dialogs.Statement;
using Documentor.Presentation.Views.Dialogs.Common;
using Documentor.Presentation.Views.Dialogs.Statement;
using Microsoft.Win32;

namespace Documentor.Presentation.ViewModels.Pages;

public class TemplatesPageViewModel : PagedListPageViewModel<TemplateListItemModel, TemplateFilterModel>
{
    private readonly ITemplateManagementService _templateManagementService;
    private readonly IAppSettingsService _appSettingsService;
    
    private readonly TemplateType _templateType;
    private List<TemplateListItemModel> _selectedTemplates = new();
    
    public IReadOnlyList<TemplateListItemModel> SelectedTemplates => _selectedTemplates;

    public bool IsMultipleSelection => _selectedTemplates.Count > 1;

    public bool IsSingleSelection => _selectedTemplates.Count == 1;

    public ObservableCollection<TemplateListItemModel> Statements => Items;

    public TemplateListItemModel? SelectedTemplate
    {
        get => SelectedItem;
        set => SelectedItem = value;
    }
    
    public string PageTitle => _templateType switch
    {
        TemplateType.Statement => "Шаблоны заявлений",
        TemplateType.Contract => "Шаблоны договоров",
        _ => "Шаблоны"
    };
    
    public PageKey PageKey { get; }

    public override string ActiveFilterSummary => _BuildFilterSummary();

    public ICommand OpenFilterCommand { get; }
    public ICommand AddTemplateCommand { get; }
    public ICommand ChangeStatusCommand { get; }
    public ICommand DeleteTemplateCommand { get; }
    public ICommand FillTemplateCommand { get; }
    public ICommand DownloadTemplateCommand { get; }
    public ICommand EditTemplateCommand { get; }
    

    public TemplatesPageViewModel(
        TemplateType templateType,
        PageKey pageKey,
        ITemplateManagementService templateManagementService,
        IAppSettingsService appSettingsService)
    {
        _templateType = templateType;
        PageKey = pageKey;
        _templateManagementService = templateManagementService;
        _appSettingsService = appSettingsService;

        InitializePageSize(_appSettingsService.GetPageSize());

        CurrentFilter = new TemplateFilterModel
        {
            PageNumber = 1,
            PageSize = PageSize,
            Type = _templateType
        };

        OpenFilterCommand = new RelayCommand(_OpenFilter);

        AddTemplateCommand = new RelayCommand(_AddTemplate);
        
        ChangeStatusCommand = new AsyncRelayCommand(
            _ChangeStatusAsync,
            () => IsSingleSelection);

        DeleteTemplateCommand = new AsyncRelayCommand(
            _DeleteTemplatesAsync,
            () => _selectedTemplates.Count > 0);

        FillTemplateCommand = new RelayCommand(
            _FillTemplate,
            () => IsSingleSelection && _selectedTemplates[0].IsActive);

        DownloadTemplateCommand = new AsyncRelayCommand(
            _DownloadAsync,
            () => IsSingleSelection && _selectedTemplates[0].IsActive);

        EditTemplateCommand = new RelayCommand(
            _EditTemplate,
            () => IsSingleSelection);

        _ = LoadAsync();
    }
    
    public void UpdateSelection(List<TemplateListItemModel> selected)
    {
        _selectedTemplates = selected;

        SelectedTemplate = IsSingleSelection ? _selectedTemplates[0] : null;

        OnPropertyChanged(nameof(IsMultipleSelection));
        OnPropertyChanged(nameof(IsSingleSelection));

        RaiseSelectionCommands();
    }
    
    public void ApplySorting(string sortMemberPath, bool descending)
    {
        CurrentFilter.SortBy = sortMemberPath switch
        {
            nameof(TemplateListItemModel.Title) => TemplateSortField.Title,
            nameof(TemplateListItemModel.CreatedBy) => TemplateSortField.CreatedBy,
            nameof(TemplateListItemModel.CreatedAt) => TemplateSortField.CreatedAt,
            nameof(TemplateListItemModel.IsActive) => TemplateSortField.IsActive,
            _ => TemplateSortField.CreatedAt
        };

        CurrentFilter.Descending = descending;
        CurrentPage = 1;

        _ = LoadAsync();
    }

    protected override void ApplyPagingToFilter()
    {
        CurrentFilter.Type = _templateType;
        CurrentFilter.PageNumber = CurrentPage;
        CurrentFilter.PageSize = PageSize;
    }

    protected override async Task<PagedResult<TemplateListItemModel>> LoadPageAsync()
    {
        return await _templateManagementService.GetTemplatesAsync(CurrentFilter);
    }

    protected override async Task ClearFilterAsync()
    {
        var pageSize = _appSettingsService.GetPageSize();

        CurrentFilter = new TemplateFilterModel
        {
            PageNumber = 1,
            PageSize = pageSize,
            Type = _templateType
        };

        CurrentPage = 1;
        PageSize = pageSize;

        await LoadAsync();
    }

    protected override void RaiseSelectionCommands()
    {
        if (ChangeStatusCommand is AsyncRelayCommand changeStatus)
            changeStatus.RaiseCanExecuteChanged();

        if (DeleteTemplateCommand is AsyncRelayCommand delete)
            delete.RaiseCanExecuteChanged();

        if (FillTemplateCommand is RelayCommand fill)
            fill.RaiseCanExecuteChanged();

        if (DownloadTemplateCommand is AsyncRelayCommand download)
            download.RaiseCanExecuteChanged();
        
        if (EditTemplateCommand is RelayCommand edit)
            edit.RaiseCanExecuteChanged();
    }

    protected override string BuildLoadErrorMessage(Exception ex)
    {
        return $"Ошибка загрузки шаблонов заявлений: {ex.Message}";
    }

    private void _OpenFilter()
    {
        TemplateFilterDialogWindow? dialog = null;

        var vm = new TemplateFilterDialogViewModel(new TemplateFilterModel
        {
            Title = CurrentFilter.Title,
            CreatedBy = CurrentFilter.CreatedBy,
            CreatedAtEarlier = CurrentFilter.CreatedAtEarlier,
            CreatedAtLater = CurrentFilter.CreatedAtLater,
            PageSize = CurrentFilter.PageSize,
            Type = _templateType
        });

        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new TemplateFilterDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            CurrentFilter = vm.ResultFilter;
            CurrentFilter.Type = _templateType;
            CurrentPage = 1;
            PageSize = CurrentFilter.PageSize ?? _appSettingsService.GetPageSize();

            _ = LoadAsync();
        }
    }

    private void _AddTemplate()
    {
        AddTemplateDialogWindow? dialog = null;

        var vm = new AddTemplateDialogViewModel(
            _templateType,
            _templateManagementService);
        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new AddTemplateDialogWindow
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
    
    private void _EditTemplate()
    {
        if (!IsSingleSelection)
            return;

        var selected = _selectedTemplates[0];

        EditTemplateDialogWindow? dialog = null;

        var vm = new EditTemplateDialogViewModel(
            _templateManagementService,
            selected.Id,
            selected);

        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new EditTemplateDialogWindow
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
        if (!IsSingleSelection)
            return;

        var selected = _selectedTemplates[0];

        try
        {
            IsLoading = true;

            var newStatus = await _templateManagementService
                .ChangeStatusAsync(selected.Id);

            selected.IsActive = newStatus;

            RaiseSelectionCommands();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task _DeleteTemplatesAsync()
    {
        if (_selectedTemplates.Count == 0)
            return;

        ConfirmationDialogWindow? dialog = null;

        var message = _selectedTemplates.Count == 1
            ? $"Удалить шаблон \"{_selectedTemplates[0].Title}\"?"
            : $"Удалить выбранные шаблоны ({_selectedTemplates.Count})?";

        var vm = new ConfirmationDialogViewModel(
            "Удаление шаблона",
            message,
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

            var ids = _selectedTemplates
                .Select(t => t.Id)
                .ToList();

            await _templateManagementService.DeleteTemplatesAsync(ids);

            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка удаления: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void _FillTemplate()
    {
        if (SelectedTemplate == null)
            return;

        FillTemplateDialogWindow? dialog = null;

        var vm = new FillTemplateDialogViewModel(
            _templateManagementService,
            SelectedTemplate.Id,
            SelectedTemplate.Title);

        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new FillTemplateDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        dialog.ShowDialog();
    }

    private async Task _DownloadAsync()
    {
        if (!IsSingleSelection)
            return;

        var selected = _selectedTemplates[0];

        try
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Word Document (*.docx)|*.docx",
                FileName = $"{selected.Title}.docx"
            };

            if (dialog.ShowDialog() != true)
                return;

            await _templateManagementService
                .DownloadTemplateAsync(selected.Id, dialog.FileName);
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