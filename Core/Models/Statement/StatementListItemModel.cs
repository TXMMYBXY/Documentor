using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Core.Models.Statement;

public class StatementListItemModel : ViewModelBase
{
    private int _id;
    private string _title;
    private string _owner;
    private DateTime _createdAt;
    private bool _isActive;
    
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Owner
    {
        get => _owner;
        set => SetProperty(ref _owner, value);
    }

    public DateTime CreatedAt
    {
        get => _createdAt.ToLocalTime();
        set => SetProperty(ref _createdAt, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }
}