using System.Windows.Input;
using DocumentFlowing.Common;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Interfaces;

namespace Documentor.Presentation.ViewModels.Dialogs.Common;

public class ApiSettingsDialogViewModel : ViewModelBase
{
    private readonly IApiEndpointProvider _apiEndpointProvider;
    private Action<bool>? _closeAction;

    private string _apiUrl = string.Empty;
    private string _errorMessage = string.Empty;

    public string ApiUrl
    {
        get => _apiUrl;
        set => SetProperty(ref _apiUrl, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand CancelCommand { get; }

    public ApiSettingsDialogViewModel(IApiEndpointProvider apiEndpointProvider)
    {
        _apiEndpointProvider = apiEndpointProvider;
        _apiUrl = _apiEndpointProvider.GetBaseUrl();

        SaveCommand = new RelayCommand(Save);
        ResetCommand = new RelayCommand(ResetToDefault);
        CancelCommand = new RelayCommand(() => _closeAction?.Invoke(false));
    }

    public void SetCloseAction(Action<bool> closeAction)
    {
        _closeAction = closeAction;
    }

    private void Save()
    {
        ErrorMessage = string.Empty;

        if (!Uri.TryCreate(ApiUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            ErrorMessage = "Введите корректный URL API.";
            return;
        }

        _apiEndpointProvider.SaveOverrideUrl(ApiUrl);
        _closeAction?.Invoke(true);
    }

    private void ResetToDefault()
    {
        _apiEndpointProvider.ClearOverrideUrl();
        ApiUrl = _apiEndpointProvider.GetBaseUrl();
        _closeAction?.Invoke(true);
    }
}