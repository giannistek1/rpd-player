using CommunityToolkit.Mvvm.ComponentModel;

namespace RpdPlayerApp.ViewModels;

internal partial class ViewModelBase : ObservableObject
{
    protected bool IsNavigating;

    /// <summary> Met deze methode navigeer je maar 1x. Voorkomt duplicate pages. </summary>
    /// <param name="route"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public async Task NavigateAsync(string route, Dictionary<string, object>? args = null)
    {
        if (IsNavigating) { return; }

        var navigationStack = Shell.Current?.Navigation?.NavigationStack;
        var currentPage = (navigationStack is not null && navigationStack.Count > 0) ? navigationStack[navigationStack.Count - 1] : null;
        var currentPageTypeName = currentPage?.GetType().Name;

        // Alleen verdergaan als je niet naar dezelfde pagina gaat.
        if (string.Equals(currentPageTypeName, route, StringComparison.OrdinalIgnoreCase)) { return; }

        Dictionary<string, object> navigationParameter = (args is null) ? [] : args;
        try
        {
            IsNavigating = true;
            await Shell.Current!.GoToAsync(route, parameters: navigationParameter);
        }
        finally
        {
            IsNavigating = false;
        }
    }
}
