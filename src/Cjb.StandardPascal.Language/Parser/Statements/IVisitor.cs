namespace Cjb.StandardPascal.Language.Parser.Statements;

public interface IVisitor<out T>
{
    T VisitBlockStatement(BlockStatement statement);

    T VisitAssignmentStatement(Assignment statement);

    T VisitPrintStatement(Print statement);

    T VisitWriteStatement(Write statement);
}