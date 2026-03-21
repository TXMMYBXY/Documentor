namespace Documentor.Presentation.Navigation;

public interface IWindowService
{
    void ShowWindow<T>() where T : System.Windows.Window;
    void ReplaceMainWindow<T>() where T : System.Windows.Window;
}