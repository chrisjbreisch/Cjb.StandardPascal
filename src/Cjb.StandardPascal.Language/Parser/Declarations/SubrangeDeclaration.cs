using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Declarations;

public sealed class SubrangeDeclaration : Declaration
{
    public SubrangeDeclaration(Token name, long minimum, long maximum, SourceSpan span) : base(span) { Name = name; Minimum = minimum; Maximum = maximum; }
    public Token Name { get; }
    public long Minimum { get; }
    public long Maximum { get; }
}