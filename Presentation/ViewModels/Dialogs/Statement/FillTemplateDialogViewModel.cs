using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Documentor.Common;
using Documentor.Core.Interfaces;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Dialogs.Statement;

public class FillTemplateDialogViewModel : DialogViewModelBase
{
    private readonly ITemplateManagementService _templateManagementService;
    private readonly int _templateId;
    private readonly string _templateTitle;

    public string TitleText => $"Заполнение шаблона: {_templateTitle}";

    public ObservableCollection<StatementDynamicFieldViewModel> Fields { get; } = new();
    // Сохраняем оригинальные поля, как их вернул бэкенд (включая суффиксы),
    // и отображаем сгруппированные по базовому имени поля в UI.
    private List<Documentor.Core.Models.Template.DynamicFieldInfoModel> _originalFields = new();
    private Dictionary<string, List<string>> _groupMap = new(StringComparer.OrdinalIgnoreCase);

    public ICommand SubmitCommand { get; }

    public FillTemplateDialogViewModel(
        ITemplateManagementService templateManagementService,
        int templateId,
        string templateTitle)
    {
        _templateManagementService = templateManagementService;
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

            var fields = (await _templateManagementService.ExtractFieldsAsync(_templateId)).ToList();

            _originalFields = fields;

            // Группируем поля по базовому имени (до ':')
            var groups = fields.GroupBy(f =>
            {
                if (string.IsNullOrWhiteSpace(f.Key)) return string.Empty;
                var idx = f.Key.IndexOf(':');
                return idx >= 0 ? f.Key.Substring(0, idx).Trim() : f.Key.Trim();
            }, StringComparer.OrdinalIgnoreCase);

            Fields.Clear();
            _groupMap.Clear();

            foreach (var g in groups)
            {
                var baseName = g.Key;

                // Запоминаем оригинальные ключи для последующей отправки
                _groupMap[baseName] = g.Select(x => x.Key).ToList();

                // Выбираем представительное поле для отображения в UI
                // Предпочитаем запись без суффикса, если есть
                var rep = g.FirstOrDefault(x => string.Equals(x.Key, baseName, StringComparison.OrdinalIgnoreCase))
                          ?? g.First();

                // Если в группе есть обязательное поле — помечаем как Required
                var required = g.Any(x => x.Required);

                var model = new Documentor.Core.Models.Template.DynamicFieldInfoModel
                {
                    Key = baseName,
                    Title = string.IsNullOrWhiteSpace(rep.Title) ? baseName : rep.Title,
                    Type = rep.Type,
                    Required = required,
                    Options = rep.Options
                };

                Fields.Add(new StatementDynamicFieldViewModel(model));
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

            // Для каждого отображаемого (сгруппированного) поля добавляем в словарь
            // значения для всех оригинальных ключей (включая суффиксы), чтобы
            // сервер мог при необходимости просклонять значения по суффиксам.
            foreach (var field in Fields)
            {
                var baseKey = field.Key;
                var value = field.GetValue();

                if (_groupMap.TryGetValue(baseKey, out var originalKeys))
                {
                    foreach (var orig in originalKeys)
                    {
                        data[orig] = value;
                    }
                }
                else
                {
                    // Непредвиденно: просто добавляем по базовому ключу
                    data[baseKey] = value;
                }
            }

            // 3) Отправка
            IsBusy = true;

            await _templateManagementService.CreateTaskAsync(_templateId, data);

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