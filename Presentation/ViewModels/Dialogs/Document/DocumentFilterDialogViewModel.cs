using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Documentor.Common;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.Document;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Dialogs.Document;

public class DocumentFilterDialogViewModel : DialogViewModelBase
{
    private readonly ITemplateManagementService _templateManagementService;

    private string _title = string.Empty;
    private DateTime? _createdAtEarlier;
    private DateTime? _createdAtLater;
    private int _pageSize = 10;
    private LookupItemModel? _selectedDocument;
    private DocumentSortField? _sortBy;
    private bool _descending;

    public DocumentSortField? SortBy
    {
        get => _sortBy;
        set => SetProperty(ref _sortBy, value);
    }

    public bool Descending
    {
        get => _descending;
        set => SetProperty(ref _descending, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public int? TemplateId => SelectedDocument?.Id == 0 ? null : SelectedDocument?.Id;

    public DateTime? CreatedAtEarlier
    {
        get => _createdAtEarlier;
        set => SetProperty(ref _createdAtEarlier, value);
    }

    public DateTime? CreatedAtLater
    {
        get => _createdAtLater;
        set => SetProperty(ref _createdAtLater, value);
    }

    public int PageSize
    {
        get => _pageSize;
        set => SetProperty(ref _pageSize, value);
    }

    public int PageNumber { get; set; }

    public LookupItemModel? SelectedDocument
    {
        get => _selectedDocument;
        set => SetProperty(ref _selectedDocument, value);
    }

    public ObservableCollection<LookupItemModel> Documents { get; } = new();
    public DocumentFilterModel ResultFilter { get; private set; } = new();

    public ICommand ApplyCommand { get; }
    public ICommand ResetCommand { get; }

    public DocumentFilterDialogViewModel(
        ITemplateManagementService templateManagementService,
        DocumentFilterModel filter)
    {
        _templateManagementService = templateManagementService;

        // initialize from current filter
        _sortBy = filter?.SortBy;
        _descending = filter?.Descending ?? false;
        _title = filter?.Title ?? string.Empty;
        _createdAtEarlier = filter?.CreatedAtEarlier;
        _createdAtLater = filter?.CreatedAtLater;
        _pageSize = filter?.PageSize == 0 ? 10 : filter?.PageSize ?? 10;

        ApplyCommand = new RelayCommand(Apply);
        ResetCommand = new RelayCommand(Reset);

        _ = _LoadLookupsAsync(filter);
    }

    private void Apply()
    {
        ResultFilter = new DocumentFilterModel
        {
            SortBy = SortBy ?? DocumentSortField.CreatedAt,
            Descending = Descending,
            Title = string.IsNullOrWhiteSpace(Title) ? null : Title,
            TemplateId = TemplateId,
            CreatedAtEarlier = CreatedAtEarlier,
            CreatedAtLater = CreatedAtLater,
            PageNumber = 1,
            PageSize = PageSize <= 0 ? 10 : PageSize
        };

        RequestClose(true);
    }

    private async Task _LoadLookupsAsync(DocumentFilterModel? currentFilter)
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var templates = await _templateManagementService.GetTemplatesAsync();

            Documents.Clear();
            Documents.Add(new LookupItemModel { Id = 0, Title = "Все" });
            foreach (var item in templates)
                Documents.Add(item);

            if (currentFilter?.TemplateId != null)
                SelectedDocument = Documents.FirstOrDefault(x => x.Id == currentFilter.TemplateId);
            else
                SelectedDocument = Documents.FirstOrDefault();
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

    private void Reset()
    {
        Title = string.Empty;
        SelectedDocument = Documents.FirstOrDefault();
        CreatedAtEarlier = null;
        CreatedAtLater = null;
        PageSize = 10;
        SortBy = null;
        Descending = false;
    }
}
