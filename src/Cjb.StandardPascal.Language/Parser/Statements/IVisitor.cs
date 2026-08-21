namespace Cjb.StandardPascal.Language.Parser.Statements;

public interface IVisitor<out T>
{
    T VisitBlockStatement(BlockStatement statement);

    T VisitAssignmentStatement(Assignment statement);

    T VisitAllocationStatement(Allocation statement);

    T VisitCaseStatement(Case statement);

    T VisitForStatement(For statement);

    T VisitGotoStatement(Goto statement);

    T VisitIfStatement(If statement);

    T VisitIndexedAssignmentStatement(IndexedAssignment statement);

    T VisitDereferenceAssignmentStatement(DereferenceAssignment statement);

    T VisitFieldAssignmentStatement(FieldAssignment statement);

    T VisitLabeledStatement(Labeled statement);

    T VisitPrintStatement(Print statement);

    T VisitProcedureCallStatement(ProcedureCall statement);

    T VisitReadStatement(Read statement);

    T VisitRepeatStatement(Repeat statement);

    T VisitWhileStatement(While statement);

    T VisitWithStatement(With statement);

    T VisitWriteStatement(Write statement);
}