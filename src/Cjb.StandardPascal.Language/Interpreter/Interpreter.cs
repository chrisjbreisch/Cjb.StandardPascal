using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics;

namespace Cjb.StandardPascal.Language.Interpreter;

public sealed class Interpreter : IInterpreter
{
    private readonly ISemanticAnalyzer _semanticAnalyzer;

    public Interpreter()
        : this(new SemanticAnalyzer())
    {
    }

    public Interpreter(ISemanticAnalyzer semanticAnalyzer)
    {
        _semanticAnalyzer = semanticAnalyzer
            ?? throw new ArgumentNullException(nameof(semanticAnalyzer));
    }

    public object Evaluate(Expression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        return expression.Accept(this);
    }

    public object Interpret(IStatement statement)
    {
        ArgumentNullException.ThrowIfNull(statement);
        return statement.Accept(this);
    }

    public object Execute(Program program)
    {
        ArgumentNullException.ThrowIfNull(program);
        _semanticAnalyzer.Analyze(program);
        return Interpret(program.Body);
    }

    public object VisitBinaryExpression(Binary expression)
    {
        object left = Evaluate(expression.Left);
        object right = Evaluate(expression.Right);
        Token binaryOperator = expression.BinaryOperator;

        try
        {
            return binaryOperator.Type switch
            {
                TokenType.Plus => Add(binaryOperator, left, right),
                TokenType.Minus => Subtract(binaryOperator, left, right),
                TokenType.Star => Multiply(binaryOperator, left, right),
                TokenType.Slash => Divide(binaryOperator, left, right),
                TokenType.Div => IntegerDivide(binaryOperator, left, right),
                TokenType.Mod => Modulo(binaryOperator, left, right),
                TokenType.And => Boolean(binaryOperator, left, right, static (x, y) => x && y),
                TokenType.Or => Boolean(binaryOperator, left, right, static (x, y) => x || y),
                TokenType.Equal => Compare(binaryOperator, left, right) == 0,
                TokenType.NotEqual => Compare(binaryOperator, left, right) != 0,
                TokenType.LessThan => Compare(binaryOperator, left, right) < 0,
                TokenType.LessThanOrEqual => Compare(binaryOperator, left, right) <= 0,
                TokenType.GreaterThan => Compare(binaryOperator, left, right) > 0,
                TokenType.GreaterThanOrEqual => Compare(binaryOperator, left, right) >= 0,
                TokenType.In => throw Error(
                    binaryOperator,
                    "Set membership is not implemented."),
                _ => throw Error(binaryOperator, "Unsupported binary operator."),
            };
        }
        catch (OverflowException)
        {
            throw Error(binaryOperator, "Integer arithmetic overflow.");
        }
    }

    public object VisitGroupingExpression(Grouping expression)
    {
        return Evaluate(expression.InnerExpression);
    }

    public object VisitIdentifierExpression(Identifier expression)
    {
        if (string.Equals(
                expression.Name.Lexeme,
                "true",
                StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (string.Equals(
                expression.Name.Lexeme,
                "false",
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        throw Error(
            expression.Name,
            $"Undefined identifier '{expression.Name.Lexeme}'.");
    }

    public object VisitLiteralExpression(Literal expression)
    {
        return expression.Value;
    }

    public object VisitUnaryExpression(Unary expression)
    {
        object right = Evaluate(expression.Right);

        try
        {
            return expression.UnaryOperator.Type switch
            {
                TokenType.Plus => RequireNumber(expression.UnaryOperator, right),
                TokenType.Minus => Negate(expression.UnaryOperator, right),
                TokenType.Not => !RequireBoolean(expression.UnaryOperator, right),
                _ => throw Error(expression.UnaryOperator, "Unsupported unary operator."),
            };
        }
        catch (OverflowException)
        {
            throw Error(expression.UnaryOperator, "Integer arithmetic overflow.");
        }
    }

    public object VisitPrintStatement(Print statement)
    {
        return Evaluate(statement.Expression);
    }

    public object VisitBlockStatement(BlockStatement statement)
    {
        object result = string.Empty;

        foreach (IStatement nestedStatement in statement.Block.Statements)
        {
            result = Interpret(nestedStatement);
        }

        return result;
    }

    private static object Add(Token token, object left, object right)
    {
        return Numeric(
            token,
            left,
            right,
            static (x, y) => checked(x + y),
            static (x, y) => x + y);
    }

    private static object Subtract(Token token, object left, object right)
    {
        return Numeric(
            token,
            left,
            right,
            static (x, y) => checked(x - y),
            static (x, y) => x - y);
    }

    private static object Multiply(Token token, object left, object right)
    {
        return Numeric(
            token,
            left,
            right,
            static (x, y) => checked(x * y),
            static (x, y) => x * y);
    }

    private static object Divide(Token token, object left, object right)
    {
        double leftNumber = ToDouble(token, left);
        double rightNumber = ToDouble(token, right);

        if (rightNumber == 0)
        {
            throw Error(token, "Division by zero.");
        }

        return leftNumber / rightNumber;
    }

    private static object IntegerDivide(Token token, object left, object right)
    {
        long leftInteger = RequireInteger(token, left);
        long rightInteger = RequireInteger(token, right);

        if (rightInteger == 0)
        {
            throw Error(token, "Division by zero.");
        }

        return checked(leftInteger / rightInteger);
    }

    private static object Modulo(Token token, object left, object right)
    {
        long leftInteger = RequireInteger(token, left);
        long rightInteger = RequireInteger(token, right);

        if (rightInteger == 0)
        {
            throw Error(token, "Division by zero.");
        }

        return leftInteger % rightInteger;
    }

    private static object Boolean(
        Token token,
        object left,
        object right,
        Func<bool, bool, bool> operation)
    {
        return operation(
            RequireBoolean(token, left),
            RequireBoolean(token, right));
    }

    private static int Compare(Token token, object left, object right)
    {
        if (left is long leftInteger && right is long rightInteger)
        {
            return leftInteger.CompareTo(rightInteger);
        }

        if (IsNumber(left) && IsNumber(right))
        {
            return ToDouble(token, left).CompareTo(ToDouble(token, right));
        }

        if (left is bool leftBoolean && right is bool rightBoolean)
        {
            return leftBoolean.CompareTo(rightBoolean);
        }

        if (left is string leftString && right is string rightString)
        {
            return string.CompareOrdinal(leftString, rightString);
        }

        throw Error(token, "Operands are not comparable.");
    }

    private static object Numeric(
        Token token,
        object left,
        object right,
        Func<long, long, long> integerOperation,
        Func<double, double, double> realOperation)
    {
        if (left is long leftInteger && right is long rightInteger)
        {
            return integerOperation(leftInteger, rightInteger);
        }

        if (IsNumber(left) && IsNumber(right))
        {
            return realOperation(ToDouble(token, left), ToDouble(token, right));
        }

        throw Error(token, "Operands must be numeric.");
    }

    private static object RequireNumber(Token token, object value)
    {
        return IsNumber(value)
            ? value
            : throw Error(token, "Operand must be numeric.");
    }

    private static object Negate(Token token, object value)
    {
        if (value is long integer)
        {
            return checked(-integer);
        }

        if (value is double real)
        {
            return -real;
        }

        throw Error(token, "Operand must be numeric.");
    }

    private static bool RequireBoolean(Token token, object value)
    {
        return value is bool boolean
            ? boolean
            : throw Error(token, "Operand must be Boolean.");
    }

    private static long RequireInteger(Token token, object value)
    {
        return value is long integer
            ? integer
            : throw Error(token, "Operands must be integers.");
    }

    private static double ToDouble(Token token, object value)
    {
        return value switch
        {
            long integer => integer,
            double real => real,
            _ => throw Error(token, "Operands must be numeric."),
        };
    }

    private static bool IsNumber(object value)
    {
        return value is long or double;
    }

    private static RuntimeException Error(Token token, string message)
    {
        return new RuntimeException(message, token.Span);
    }
}