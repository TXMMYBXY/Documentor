using System.Collections.ObjectModel;
using Documentor.Core.Models.Statement;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Dialogs.Statement;

public class StatementDynamicFieldViewModel : ViewModelBase
{
    private string? _textValue;
    private bool _boolValue;
    private DateTime? _dateValue;
    private string _validationMessage = string.Empty;

    public string Key { get; }
    public string Title { get; }
    public string Type { get; }
    public bool Required { get; }

    public ObservableCollection<string> Options { get; } = new();

    public string? TextValue
    {
        get => _textValue;
        set => SetProperty(ref _textValue, value);
    }

    public bool BoolValue
    {
        get => _boolValue;
        set => SetProperty(ref _boolValue, value);
    }

    public DateTime? DateValue
    {
        get => _dateValue;
        set => SetProperty(ref _dateValue, value);
    }

    public string ValidationMessage
    {
        get => _validationMessage;
        set => SetProperty(ref _validationMessage, value);
    }

    public bool IsTextField => Type is "string" or "text" or "richtext";
    public bool IsDateField => Type == "date";
    public bool IsCheckboxField => Type == "checkbox";
    public bool IsSelectField => Type is "dropdown" or "combobox";
    public bool IsUnsupported => !(IsTextField || IsDateField || IsCheckboxField || IsSelectField);

    public StatementDynamicFieldViewModel(DynamicFieldInfoModel model)
    {
        Key = model.Key;
        Title = string.IsNullOrWhiteSpace(model.Title) ? model.Key : model.Title;
        Type = model.Type?.Trim().ToLowerInvariant() ?? "string";
        Required = model.Required;

        if (model.Options != null)
        {
            foreach (var option in model.Options)
                Options.Add(option);
        }
    }

    public bool Validate()
    {
        ValidationMessage = string.Empty;

        if (IsUnsupported)
        {
            ValidationMessage = $"Тип поля \"{Title}\" пока не поддерживается.";
            return false;
        }

        if (!Required)
            return true;

        var valid = Type switch
        {
            "date" => DateValue.HasValue,
            "checkbox" => true,
            _ => !string.IsNullOrWhiteSpace(TextValue)
        };

        if (!valid)
            ValidationMessage = $"Поле \"{Title}\" обязательно для заполнения.";

        return valid;
    }

    public object GetValue()
    {
        return Type switch
        {
            "date" => DateValue?.ToString("yyyy-MM-dd") ?? string.Empty,
            "checkbox" => BoolValue,
            _ => TextValue ?? string.Empty
        };
    }
}