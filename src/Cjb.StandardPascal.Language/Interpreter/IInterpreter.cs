using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Parser.Statements;

namespace Cjb.StandardPascal.Language.Interpreter;

public interface IInterpreter :
    Parser.Expressions.IVisitor<object>,
    Parser.Statements.IVisitor<object>
{
    object Evaluate(Expression expression);

    object Interpret(IStatement statement);
}