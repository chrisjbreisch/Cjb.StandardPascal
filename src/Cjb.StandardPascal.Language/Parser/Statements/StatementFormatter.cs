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

    public string VisitBlockStatement(BlockStatement statement)
    {
        return "(block)";
    }

    public string VisitAssignmentStatement(Assignment statement) => "(:=)";

    public string VisitForStatement(For statement) => "(for)";

    public string VisitIfStatement(If statement) => "(if)";

    public string VisitWriteStatement(Write statement) => "(write)";

    public string VisitRepeatStatement(Repeat statement) => "(repeat)";

    public string VisitWhileStatement(While statement) => "(while)";
}