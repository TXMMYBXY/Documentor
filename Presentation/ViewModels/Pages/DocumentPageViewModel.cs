using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Documentor.Common;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.Document;
using Documentor.Core.Models.Statement;
using Documentor.Presentation.ViewModels.Base;
using Documentor.Presentation.ViewModels.Dialogs.Common;
using Documentor.Presentation.ViewModels.Dialogs.Document;
using Documentor.Presentation.ViewModels.Dialogs.Statement;
using Documentor.Presentation.Views.Dialogs.Common;
using Documentor.Presentation.Views.Dialogs.Document;
using Microsoft.Win32;

namespace Documentor.Presentation.ViewModels.Pages;

public class DocumentPageViewModel : PagedListPageViewModel<DocumentListItemModel, DocumentFilterModel>
{
    private readonly IDocumentManagementService _documentManagementService;
    private readonly ITemplateManagementService _templateManagementService;
    private readonly IAppSettingsService _appSettingsService;

    public static string Title => "Архив документов";

    public ObservableCollection<DocumentListItemModel> Documents => Items;

    public DocumentListItemModel? SelectedDocument
    {
        get => SelectedItem;
        set => SelectedItem = value;
    }

    public override string ActiveFilterSummary => _BuildFilterSummary();

    public ICommand OpenFilterCommand { get; }
    public ICommand DeleteDocumentCommand { get; }
    public ICommand DownloadDocumentCommand { get; }
    

    public DocumentPageViewModel(
        IDocumentManagementService documentManagementService,
        ITemplateManagementService templateManagementService,
        IAppSettingsService appSettingsService)
    {
        _documentManagementService = documentManagementService;
        _templateManagementService = templateManagementService;
        _appSettingsService = appSettingsService;

        InitializePageSize(_appSettingsService.GetPageSize());

        CurrentFilter = new DocumentFilterModel
        {
            PageNumber = 1,
            PageSize = PageSize
        };

        OpenFilterCommand = new RelayCommand(_OpenFilter);
        DeleteDocumentCommand = new AsyncRelayCommand(_DeleteDocumentAsync, () => SelectedDocument != null);
        DownloadDocumentCommand = new AsyncRelayCommand(_DownloadDocumentAsync);

        _ = LoadAsync();
    }

    protected override void ApplyPagingToFilter()
    {
        CurrentFilter.PageNumber = CurrentPage;
        CurrentFilter.PageSize = PageSize;
    }

    protected override async Task<PagedResult<DocumentListItemModel>> LoadPageAsync()
    {
        return await _documentManagementService.GetAllDocumentsAsync(CurrentFilter);
    }

    protected override async Task ClearFilterAsync()
    {
        var pageSize = _appSettingsService.GetPageSize();

        CurrentFilter = new DocumentFilterModel
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
        if (DeleteDocumentCommand is AsyncRelayCommand delete)
            delete.RaiseCanExecuteChanged();

        if (DownloadDocumentCommand is AsyncRelayCommand download)
            download.RaiseCanExecuteChanged();
    }

    protected override string BuildLoadErrorMessage(Exception ex)
    {
        return $"Ошибка загрузки шаблонов заявлений: {ex.Message}";
    }

    private void _OpenFilter()
    {
        DocumentFilterDialogWindow? dialog = null;

        var vm = new DocumentFilterDialogViewModel(_templateManagementService, 
            new DocumentFilterModel
        {
            Title = CurrentFilter.Title,
            CreatedAtEarlier = CurrentFilter.CreatedAtEarlier,
            CreatedAtLater = CurrentFilter.CreatedAtLater,
            PageSize = CurrentFilter.PageSize
        });

        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new DocumentFilterDialogWindow
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
    
    private async Task _DeleteDocumentAsync()
    {
        if (SelectedDocument == null)
            return;

        ConfirmationDialogWindow? dialog = null;

        var vm = new ConfirmationDialogViewModel(
            "Удаление шаблона заявления",
            $"Удалить шаблон \"{SelectedDocument.Title}\"?",
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

            await _documentManagementService.DeleteDocumentAsync(SelectedDocument.Id);

            if (Documents.Count == 1 && CurrentPage > 1)
            {
                CurrentPage--;
            }

            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка удаления документа: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task _DownloadDocumentAsync()
    {
        if (SelectedDocument == null)
            return;

        try
        {
            ErrorMessage = string.Empty;

            var dialog = new SaveFileDialog
            {
                Filter = "Word Document (*.docx)|*.docx",
                FileName = $"{SelectedDocument.Title}.docx"
            };

            if (dialog.ShowDialog() != true)
                return;

            await _documentManagementService.DownloadDocumentAsync(SelectedDocument.Id, dialog.FileName);

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