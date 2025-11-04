namespace RpdPlayerApp.Architecture;

internal class InputPromptResult
{
    internal string Text { get; set; }
    internal bool IsCanceled { get; set; }
    internal InputPromptResult(string resultText = "", bool isCanceled = true)
    {
        Text = resultText;
        IsCanceled = isCanceled;
    }
}
