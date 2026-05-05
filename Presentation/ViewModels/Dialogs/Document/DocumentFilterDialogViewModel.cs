using System.Windows.Input;
using Documentor.Common;
using Documentor.Core.Models.Document;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Dialogs.Document;

public class DocumentFilterDialogViewModel: DialogViewModelBase
{
    private string _title = string.Empty;
    private int _pageSize = 10;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public int PageSize
    {
        get => _pageSize;
        set => SetProperty(ref _pageSize, value);
    }

    public DocumentFilterModel ResultFilter { get; private set; } = new();

    public ICommand ApplyCommand { get; }
    public ICommand ResetCommand { get; }

    public DocumentFilterDialogViewModel(DocumentFilterModel filter)
    {
        _title = filter.Title;
        _pageSize = filter.PageSize == 0 ? 10 : filter.PageSize.Value;

        ApplyCommand = new RelayCommand(Apply);
        ResetCommand = new RelayCommand(Reset);
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

    private void Reset()
    {
        Title = string.Empty;
        PageSize = 10;
    }
}