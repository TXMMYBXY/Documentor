using System.Windows.Input;
using Documentor.Common;
using Documentor.Core.Models.Statement;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Dialogs.Statement;

public class StatementFilterDialogViewModel : DialogViewModelBase
{
    private string _title = string.Empty;
    private string _createdBy = string.Empty;
    private DateTime? _createdAtEarlier;
    private DateTime? _createdAtLater;
    private int _pageSize = 10;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string CreatedBy
    {
        get => _createdBy;
        set => SetProperty(ref _createdBy, value);
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

    public StatementFilterModel ResultFilter { get; private set; } = new();

    public ICommand ApplyCommand { get; }
    public ICommand ResetCommand { get; }

    public StatementFilterDialogViewModel(StatementFilterModel filter)
    {
        Title = filter.Title ?? string.Empty;
        CreatedBy = filter.CreatedBy?.ToString() ?? string.Empty;
        CreatedAtEarlier = filter.CreatedAtEarlier;
        CreatedAtLater = filter.CreatedAtLater;
        PageSize = filter.PageSize.Value;

        ApplyCommand = new RelayCommand(Apply);
        ResetCommand = new RelayCommand(Reset);
    }

    private void Apply()
    {
        int? createdBy = null;
        if (int.TryParse(CreatedBy, out var parsed))
            createdBy = parsed;

        ResultFilter = new StatementFilterModel
        {
            Title = string.IsNullOrWhiteSpace(Title) ? null : Title,
            CreatedBy = createdBy,
            CreatedAtEarlier = CreatedAtEarlier,
            CreatedAtLater = CreatedAtLater,
            PageNumber = 1,
            PageSize = PageSize <= 0 ? 10 : PageSize
        };

        RequestClose(true);
    }

    private void Reset()
    {
        Title = string.Empty;
        CreatedBy = string.Empty;
        CreatedAtEarlier = null;
        CreatedAtLater = null;
        PageSize = 10;
    }
}