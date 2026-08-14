namespace Cjb.StandardPascal.Language.Scanner;

public readonly record struct SourceSpan(
    string FilePath,
    int Start,
    int Length,
    int Line,
    int Column);