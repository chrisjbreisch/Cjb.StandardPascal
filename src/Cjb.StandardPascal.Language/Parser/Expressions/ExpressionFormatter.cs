using System.Globalization;

namespace Cjb.StandardPascal.Language.Parser.Expressions;

public sealed class ExpressionFormatter : IExpressionFormatter, IVisitor<string>
{
    public string VisitCallExpression(Call expression) => $"({expression.Name.Lexeme})";

    public string VisitDereferenceExpression(Dereference expression) => "(^)";

    public string VisitFieldExpression(Field expression) => "(.)";

    public string VisitIndexExpression(Index expression) => "(index)";

    public string VisitSetLiteralExpression(SetLiteral expression) => "(set)";

    public string VisitSetRangeExpression(SetRange expression) => "(set range)";

    public string Format(Expression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        return expression.Accept(this);
    }

    public string VisitBinaryExpression(Binary expression)
    {
        return Parenthesize(
            expression.BinaryOperator.Lexeme.ToLowerInvariant(),
            expression.Left,
            expression.Right);
    }

    public string VisitGroupingExpression(Grouping expression)
    {
        return Parenthesize("group", expression.InnerExpression);
    }

    public string VisitIdentifierExpression(Identifier expression)
    {
        return expression.Name.Lexeme;
    }

    public string VisitLiteralExpression(Literal expression)
    {
        return Convert.ToString(expression.Value, CultureInfo.InvariantCulture)
            ?? string.Empty;
    }

    public string VisitUnaryExpression(Unary expression)
    {
        return Parenthesize(
            expression.UnaryOperator.Lexeme.ToLowerInvariant(),
            expression.Right);
    }

    private string Parenthesize(string name, params Expression[] expressions)
    {
        IEnumerable<string> operands = expressions.Select(
            expression => expression.Accept(this));
        return $"({name} {string.Join(' ', operands)})";
    }
}