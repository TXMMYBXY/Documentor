using System.Collections.ObjectModel;
using System.Windows.Input;
using DocumentFlowing.Common;
using Documentor.Core.Interfaces;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Dialogs.Statement;

public class FillStatementTemplateDialogViewModel : DialogViewModelBase
{
    private readonly IStatementManagementService _statementManagementService;
    private readonly int _templateId;
    private readonly string _templateTitle;

    public string TitleText => $"Заполнение шаблона: {_templateTitle}";

    public ObservableCollection<StatementDynamicFieldViewModel> Fields { get; } = new();

    public ICommand SubmitCommand { get; }

    public FillStatementTemplateDialogViewModel(
        IStatementManagementService statementManagementService,
        int templateId,
        string templateTitle)
    {
        _statementManagementService = statementManagementService;
        _templateId = templateId;
        _templateTitle = templateTitle;

        SubmitCommand = new RelayCommand(_Submit);

        _ = _LoadAsync();
    }

    private async Task _LoadAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var fields = await _statementManagementService.ExtractFieldsAsync(_templateId);

            Fields.Clear();
            foreach (var field in fields)
            {
                Fields.Add(new StatementDynamicFieldViewModel(field));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка загрузки полей шаблона: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void _Submit()
    {
        try
        {
            ErrorMessage = string.Empty;

            // 1) Валидация
            var isValid = true;
            foreach (var field in Fields)
            {
                if (!field.Validate())
                    isValid = false;
            }

            if (!isValid)
            {
                ErrorMessage = "Заполните обязательные поля.";
                return;
            }

            // 2) Сбор данных
            var data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            foreach (var field in Fields)
            {
                // если ключи вдруг повторяются — чтобы не падало:
                data[field.Key] = field.GetValue();
            }

            // 3) Отправка
            IsBusy = true;

            await _statementManagementService.CreateTaskAsync(_templateId, data);

            RequestClose(true);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка отправки данных: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}