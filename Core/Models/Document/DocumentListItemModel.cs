using Documentor.Application.Api.Statement.Dtos;
using Documentor.Core.Enums;
using Documentor.Core.Extensions;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Core.Models.Document;

public class DocumentListItemModel : ViewModelBase
{
    private int _id;
    private string _title;
    private DateTime _createdAt;
    private TemplateType _type;
    private string _templateTitle;

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

    public DateTime CreatedAt
    {
        get => _createdAt.ToLocalTime();
        set => SetProperty(ref _createdAt, value);
    }

    public TemplateType Type
    {
        get => _type;
        set => SetProperty(ref _type, value);
    }
    
    public string TypeDisplay => Type.GetDisplayName();

    public string TemplateTitle
    {
        get => _templateTitle;
        set => SetProperty(ref _templateTitle, value);
    }
}