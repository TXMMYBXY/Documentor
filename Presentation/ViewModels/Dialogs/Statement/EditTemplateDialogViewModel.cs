using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Documentor.Common;
using Documentor.Core.Interfaces;
using Documentor.Core.Models.Template;
using Documentor.Presentation.ViewModels.Base;
using Microsoft.Win32;

namespace Documentor.Presentation.ViewModels.Dialogs.Statement;

public class EditTemplateDialogViewModel : DialogViewModelBase
{
    private readonly ITemplateManagementService _templateManagementService;
    private readonly int _templateId;

    private readonly string _originalTitle;

    private string _title;
    private string _filePath = string.Empty;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    /// <summary>Локально выбранный файл для замены</summary>
    public string FilePath
    {
        get => _filePath;
        set => SetProperty(ref _filePath, value);
    }

    public ICommand BrowseFileCommand { get; }
    public ICommand ClearFileCommand { get; }
    public ICommand SaveCommand { get; }

    public EditTemplateDialogViewModel(
        ITemplateManagementService templateManagementService,
        int templateId,
        TemplateListItemModel template)
    {
        _templateManagementService = templateManagementService;
        _templateId = templateId;

        _originalTitle = template.Title;
        _title = template.Title;

        BrowseFileCommand = new RelayCommand(_BrowseFile);
        ClearFileCommand = new RelayCommand(() => FilePath = string.Empty);
        SaveCommand = new AsyncRelayCommand(_SaveAsync);
    }

    private void _BrowseFile()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Word Document (*.docx)|*.docx",
            CheckFileExists = true,
            Multiselect = false,
            Title = "Выберите новый файл шаблона"
        };

        if (dialog.ShowDialog() == true)
        {
            FilePath = dialog.FileName;
        }
    }

    private async Task _SaveAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var hasNewTitle = !string.IsNullOrWhiteSpace(Title) && !string.Equals(Title.Trim(), _originalTitle, StringComparison.Ordinal);
            var hasNewFile = !string.IsNullOrWhiteSpace(FilePath);

            if (!hasNewTitle && !hasNewFile)
            {
                ErrorMessage = "Нет изменений для сохранения.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Название шаблона не может быть пустым.";
                return;
            }

            if (hasNewFile && !File.Exists(FilePath))
            {
                ErrorMessage = "Выбранный файл не найден.";
                return;
            }

            await _templateManagementService.UpdateStatementTemplateAsync(
                _templateId,
                hasNewTitle ? Title.Trim() : null,
                hasNewFile ? FilePath : null);

            RequestClose(true);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка сохранения шаблона: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}