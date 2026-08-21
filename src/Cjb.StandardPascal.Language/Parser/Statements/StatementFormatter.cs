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

    public string VisitWriteStatement(Write statement) => "(write)";
}