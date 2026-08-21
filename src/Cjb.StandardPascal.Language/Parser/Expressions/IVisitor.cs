namespace Cjb.StandardPascal.Language.Parser.Expressions;

public interface IVisitor<out T>
{
    T VisitBinaryExpression(Binary expression);

    T VisitCallExpression(Call expression);

    T VisitDereferenceExpression(Dereference expression);

    T VisitGroupingExpression(Grouping expression);

    T VisitIndexExpression(Index expression);

    T VisitIdentifierExpression(Identifier expression);

    T VisitLiteralExpression(Literal expression);

    T VisitSetLiteralExpression(SetLiteral expression);

    T VisitUnaryExpression(Unary expression);
}