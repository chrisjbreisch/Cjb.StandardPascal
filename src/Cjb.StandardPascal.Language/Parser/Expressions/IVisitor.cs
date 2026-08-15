namespace Cjb.StandardPascal.Language.Parser.Expressions;

public interface IVisitor<out T>
{
    T VisitBinaryExpression(Binary expression);

    T VisitGroupingExpression(Grouping expression);

    T VisitIdentifierExpression(Identifier expression);

    T VisitLiteralExpression(Literal expression);

    T VisitUnaryExpression(Unary expression);
}