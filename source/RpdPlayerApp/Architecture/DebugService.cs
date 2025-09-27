using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;

namespace RpdPlayerApp.Architecture;

internal class DebugService : ObservableObject
{
    private static readonly Lazy<DebugService> _instance =
            new(() => new DebugService());
    public static DebugService Instance => _instance.Value;

    private readonly StringBuilder _logBuilder = new();

    public string DebugLog => _logBuilder.ToString();
    public bool DebugInfoVisible
    {
        get => Preferences.Get(key: CommonSettings.DEBUG_MODE, defaultValue: false);
        set
        {
            Preferences.Set(CommonSettings.DEBUG_MODE, value);
            OnPropertyChanged();
        }
    }

    private DebugService() { }

    public void AddDebug(string? msg = "NULL")
    {
        _logBuilder.AppendLine($"{DateTime.Now:HH:mm:ss} - {msg}");

        // Split into lines and trim to max 8
        var lines = _logBuilder.ToString()
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        if (lines.Count > 240 / 15) // Height of scrollview / approx line height
        {
            lines = lines.Skip(lines.Count - 8).ToList();
            _logBuilder.Clear();
            foreach (var line in lines)
                _logBuilder.AppendLine(line);
        }

        OnPropertyChanged(nameof(DebugLog));
    }
}