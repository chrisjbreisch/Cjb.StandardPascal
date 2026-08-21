using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Declarations;

public sealed class ConstantDeclaration : Declaration
{
    public ConstantDeclaration(Token name, Expression value, SourceSpan span)
        : base(span)
    {
        Name = name;
        Value = value;
    }

    public Token Name { get; }

    public Expression Value { get; }
}