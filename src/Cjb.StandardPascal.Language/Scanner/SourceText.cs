namespace Cjb.StandardPascal.Language.Scanner;

public sealed class SourceText
{
    public SourceText(string text, string filePath = "")
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public string FilePath { get; }

    public string Text { get; }
}