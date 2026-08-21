using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Declarations;

public sealed class EnumerationDeclaration : Declaration
{
    public EnumerationDeclaration(Token name, IReadOnlyList<Token> members, SourceSpan span) : base(span) { Name = name; Members = members.ToArray(); }
    public Token Name { get; }
    public IReadOnlyList<Token> Members { get; }
}