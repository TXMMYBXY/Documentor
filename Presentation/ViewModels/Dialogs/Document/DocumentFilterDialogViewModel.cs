using System.Collections.ObjectModel;
using System.Windows.Input;
using Documentor.Common;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.Document;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Dialogs.Document;

public class DocumentFilterDialogViewModel: DialogViewModelBase
{
    private readonly ITemplateManagementService _templateManagementService;
    
    private string _title = string.Empty;
    private DateTime? _createdAtEarlier;
    private DateTime? _createdAtLater;
    private int _pageSize = 10;
    
    private LookupItemModel? _selectedDocument;
    

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    
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
        
        _title = filter.Title;
        _pageSize = filter.PageSize == 0 ? 10 : filter.PageSize.Value;

        ApplyCommand = new RelayCommand(Apply);
        ResetCommand = new RelayCommand(_Reset);
    }

    private void Apply()
    {
        ResultFilter = new DocumentFilterModel
        {
            Title = Title,
            PageSize = PageSize <= 0 ? 10 : PageSize,
            PageNumber = 1
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

    private void _Reset()
    {
        Title = string.Empty;
        PageSize = 10;
    }
}