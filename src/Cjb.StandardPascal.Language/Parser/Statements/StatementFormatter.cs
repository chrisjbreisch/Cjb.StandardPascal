using Cjb.StandardPascal.Language.Parser.Expressions;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class StatementFormatter : IStatementFormatter, IVisitor<string>
{
    private readonly IExpressionFormatter _expressionFormatter;

    public StatementFormatter(IExpressionFormatter expressionFormatter)
    {
        _expressionFormatter = expressionFormatter
            ?? throw new ArgumentNullException(nameof(expressionFormatter));
    }

    public string Format(IStatement statement)
    {
        ArgumentNullException.ThrowIfNull(statement);
        return statement.Accept(this);
    }

    public string VisitPrintStatement(Print statement)
    {
        return $"(print {_expressionFormatter.Format(statement.Expression)})";
    }

    public string VisitProcedureCallStatement(ProcedureCall statement) => "(call)";

    public string VisitReadStatement(Read statement) => "(read)";

    public string VisitBlockStatement(BlockStatement statement)
    {
        return "(block)";
    }

    public string VisitAssignmentStatement(Assignment statement) => "(:=)";

    public string VisitAllocationStatement(Allocation statement) => "(allocation)";

    public string VisitCaseStatement(Case statement) => "(case)";

    public string VisitForStatement(For statement) => "(for)";

    public string VisitGotoStatement(Goto statement) => "(goto)";

    public string VisitIfStatement(If statement) => "(if)";

    public string VisitIndexedAssignmentStatement(IndexedAssignment statement) => "(index :=)";

    public string VisitDereferenceAssignmentStatement(DereferenceAssignment statement) => "(^ :=)";

    public string VisitLabeledStatement(Labeled statement) => "(label)";

    public string VisitWriteStatement(Write statement) => "(write)";

    public string VisitRepeatStatement(Repeat statement) => "(repeat)";

    public string VisitWhileStatement(While statement) => "(while)";

    public string VisitWithStatement(With statement) => "(with)";
}