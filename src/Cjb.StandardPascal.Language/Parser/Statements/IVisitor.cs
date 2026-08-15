namespace Cjb.StandardPascal.Language.Parser.Statements;

public interface IVisitor<out T>
{
    T VisitPrintStatement(Print statement);
}