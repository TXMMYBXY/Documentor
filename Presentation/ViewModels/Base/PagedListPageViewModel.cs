using System.Collections.ObjectModel;
using System.Windows.Input;
using Documentor.Common;
using Documentor.Core.Models;

namespace Documentor.Presentation.ViewModels.Base;

public abstract class PagedListPageViewModel<TItem, TFilter> : ViewModelBase
{
    private TItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalPages;
    private int _totalCount;

    protected TFilter CurrentFilter = default!;

    public ObservableCollection<TItem> Items { get; } = new();

    public TItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                RaiseSelectionCommands();
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
        set => SetProperty(ref _isLoading, value);
    }

    public int CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    public int PageSize
    {
        get => _pageSize;
        set => SetProperty(ref _pageSize, value);
    }

    public int TotalPages
    {
        get => _totalPages;
        set => SetProperty(ref _totalPages, value);
    }

    public int TotalCount
    {
        get => _totalCount;
        set => SetProperty(ref _totalCount, value);
    }

    public bool IsEmpty => Items.Count == 0;
    public bool CanGoPrevious => CurrentPage > 1;
    public bool CanGoNext => CurrentPage < TotalPages;

    public ICommand RefreshCommand { get; }
    public ICommand ClearFilterCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }

    protected PagedListPageViewModel()
    {
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        ClearFilterCommand = new AsyncRelayCommand(ClearFilterAsync);
        NextPageCommand = new AsyncRelayCommand(NextPageAsync, () => CanGoNext);
        PreviousPageCommand = new AsyncRelayCommand(PreviousPageAsync, () => CanGoPrevious);
    }

    protected void InitializePageSize(int pageSize)
    {
        PageSize = pageSize;
    }

    public async Task LoadAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            ApplyPagingToFilter();

            var result = await LoadPageAsync();

            Items.Clear();
            foreach (var item in result.Items)
            {
                Items.Add(item);
            }

            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
            CurrentPage = result.CurrentPage == 0 ? 1 : result.CurrentPage;
            PageSize = result.PageSize == 0 ? PageSize : result.PageSize;

            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(ActiveFilterSummary));

            RaisePagingStateChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = BuildLoadErrorMessage(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task NextPageAsync()
    {
        if (!CanGoNext)
            return;

        CurrentPage++;
        await LoadAsync();
    }

    protected async Task PreviousPageAsync()
    {
        if (!CanGoPrevious)
            return;

        CurrentPage--;
        await LoadAsync();
    }

    protected void RaisePagingStateChanged()
    {
        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));

        if (NextPageCommand is AsyncRelayCommand next)
            next.RaiseCanExecuteChanged();

        if (PreviousPageCommand is AsyncRelayCommand prev)
            prev.RaiseCanExecuteChanged();
    }

    public abstract string ActiveFilterSummary { get; }

    protected abstract void ApplyPagingToFilter();
    protected abstract Task<PagedResult<TItem>> LoadPageAsync();
    protected abstract Task ClearFilterAsync();
    protected abstract void RaiseSelectionCommands();
    protected abstract string BuildLoadErrorMessage(Exception ex);
}