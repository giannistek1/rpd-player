using CommunityToolkit.Mvvm.ComponentModel;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.DTO;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using System.Text;

namespace RpdPlayerApp.Services;

internal class DebugService : ObservableObject
{
    private static readonly Lazy<DebugService> _instance = new(() => new DebugService());
    public static DebugService Instance => _instance.Value;

    private readonly StringBuilder _logBuilder = new();
    private readonly StringBuilder _visibleLog = new();

    public string DebugLog => _visibleLog.ToString();
    public bool DebugInfoVisible
    {
        get => Preferences.Get(key: CommonSettings.DEBUG_MODE, defaultValue: false);
        set
        {
            Preferences.Set(CommonSettings.DEBUG_MODE, value);
            OnPropertyChanged();
        }
    }

    private const int MAX_LINES = 8;

    private DebugService() { }

    public void AddDebug(string? msg = "NULL")
    {
        _logBuilder.AppendLine($"{DateTime.Now:HH:mm:ss} - {msg}");

        // Split into lines and trim to max 8
        var lines = _logBuilder.ToString()
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        int maxLineCount = 300 / 15;

        if (lines.Count > maxLineCount) // Height of scrollview / approx line height
        {
            lines = lines.Skip(lines.Count - maxLineCount).ToList();
            _logBuilder.Clear();
            foreach (var line in lines)
                _logBuilder.AppendLine(line);
        }

        OnPropertyChanged(nameof(DebugLog));
    }

    // Shortcuts for category-based logging.
    public void Fatal(string? msg = "NULL") => AddInternal(LogCategoryValue.Fatal, msg);
    public void Error(string? msg = "NULL") => AddInternal(LogCategoryValue.Error, msg);
    public void Warn(string? msg = "NULL") => AddInternal(LogCategoryValue.Warn, msg);
    public void Info(string? msg = "NULL") => AddInternal(LogCategoryValue.Info, msg);
    public void Debug(string? msg = "NULL") => AddInternal(LogCategoryValue.Debug, msg);
    public void Trace(string? msg = "NULL") => AddInternal(LogCategoryValue.Trace, msg);

    private void AddInternal(LogCategoryValue category, string? msg)
    {
        var message = msg ?? "NULL";
        var timestamp = DateTime.Now;

        var formatted = $"{timestamp:HH:mm:ss} [{category}] - {message}";
        _visibleLog.AppendLine(formatted);

        // Split into lines and trim to max 8
        var lines = _logBuilder.ToString()
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        if (lines.Count > 240 / 15) // Height of scrollview / approx line height
        {
            lines = lines.Skip(lines.Count - MAX_LINES).ToList();
            _logBuilder.Clear();
            foreach (var line in lines)
                _logBuilder.AppendLine(line);
        }

        var entry = new LogDto
        {
            Timestamp = timestamp,
            Category = category,
            Message = message,
            DeviceId = AppState.DeviceId
        };

        _logBuilder.Append(entry.Message);

        OnPropertyChanged(nameof(DebugLog));
    }

    public async Task UploadLog()
    {
        // TODO: supabase call
    }
}