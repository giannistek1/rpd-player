
namespace RpdPlayerApp.Models;

internal class InputPromptResult
{
    internal string ResultText { get; set; }
    internal bool IsCanceled { get; set; }
    internal InputPromptResult(string resultText = "", bool isCanceled = true)
    {
        ResultText = resultText;
        IsCanceled = isCanceled;
    }
}
