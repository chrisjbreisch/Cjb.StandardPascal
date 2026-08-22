using Cjb.StandardPascal.Language.Parser.Expressions;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class WriteItem
{
    public WriteItem(Expression expression, Expression? width, Expression? precision)
    {
        Expression = expression;
        Width = width;
        Precision = precision;
    }

    public Expression Expression { get; }
    public Expression? Width { get; }
    public Expression? Precision { get; }
}