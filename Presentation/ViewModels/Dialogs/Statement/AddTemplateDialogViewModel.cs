using System.IO;
using System.Windows.Input;
using Documentor.Common;
using Documentor.Core.Enums;
using Documentor.Core.Interfaces;
using Documentor.Core.Models.Template;
using Documentor.Presentation.ViewModels.Base;
using Microsoft.Win32;

namespace Documentor.Presentation.ViewModels.Dialogs.Statement;

public class AddTemplateDialogViewModel : DialogViewModelBase
{
    private readonly ITemplateManagementService _templateManagementService;

    private string _title = string.Empty;
    private string _filePath = string.Empty;
    private TemplateType _type;
    private bool _isActive = true;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string FilePath
    {
        get => _filePath;
        set => SetProperty(ref _filePath, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public ICommand BrowseFileCommand { get; }
    public ICommand SaveCommand { get; }

    public AddTemplateDialogViewModel(TemplateType type, ITemplateManagementService templateManagementService)
    {
        _type = type;
        _templateManagementService = templateManagementService;

        BrowseFileCommand = new RelayCommand(BrowseFile);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
    }

    private void BrowseFile()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Word Document (*.docx)|*.docx",
            CheckFileExists = true,
            Multiselect = false,
            Title = "Выберите шаблон заявления"
        };

        if (dialog.ShowDialog() == true)
        {
            FilePath = dialog.FileName;

            if (string.IsNullOrWhiteSpace(Title))
            {
                Title = Path.GetFileNameWithoutExtension(dialog.FileName);
            }
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Введите название шаблона.";
                return;
            }

            if (string.IsNullOrWhiteSpace(FilePath) || !File.Exists(FilePath))
            {
                ErrorMessage = "Выберите корректный файл шаблона.";
                return;
            }

            await _templateManagementService.CreateStatementAsync(new CreateTemplateModel
            {
                Title = Title,
                IsActive = IsActive,
                Type = _type,
                FilePath = FilePath,
            });

            RequestClose(true);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка добавления шаблона: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}