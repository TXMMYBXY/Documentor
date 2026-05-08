using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Core.Models.Template;

public class TemplateListItemModel : ViewModelBase
{
    private int _id;
    private string _title;
    private string _createdBy;
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

    public string CreatedBy
    {
        get => _createdBy;
        set => SetProperty(ref _createdBy, value);
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