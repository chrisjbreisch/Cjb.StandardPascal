namespace Cjb.StandardPascal.Language.Parser.Statements;

public interface IVisitor<out T>
{
    T VisitBlockStatement(BlockStatement statement);

    T VisitAssignmentStatement(Assignment statement);

    T VisitForStatement(For statement);

    T VisitIfStatement(If statement);

    T VisitPrintStatement(Print statement);

    T VisitRepeatStatement(Repeat statement);

    T VisitWhileStatement(While statement);

    T VisitWriteStatement(Write statement);
}