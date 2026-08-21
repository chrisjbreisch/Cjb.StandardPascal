using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics.Types;

namespace Cjb.StandardPascal.Language.Semantics;

public sealed class SemanticAnalyzer : ISemanticAnalyzer
{
    public void Analyze(Program program)
    {
        ArgumentNullException.ThrowIfNull(program);
        AnalyzeStatement(program.Body);
    }

    private static void AnalyzeStatement(IStatement statement)
    {
        switch (statement)
        {
            case BlockStatement blockStatement:
                foreach (IStatement nestedStatement in blockStatement.Block.Statements)
                {
                    AnalyzeStatement(nestedStatement);
                }

                return;
            case Print print:
                InferType(print.Expression);
                return;
            default:
                throw new SemanticException("Unsupported statement.", statement.Span);
        }
    }

    private static PascalType InferType(Expression expression)
    {
        return expression switch
        {
            Literal literal => InferLiteralType(literal),
            Identifier identifier => InferIdentifierType(identifier),
            Grouping grouping => InferType(grouping.InnerExpression),
            Unary unary => InferUnaryType(unary),
            Binary binary => InferBinaryType(binary),
            _ => throw new SemanticException("Unsupported expression.", expression.Span),
        };
    }

    private static PascalType InferLiteralType(Literal literal)
    {
        return literal.Value switch
        {
            long => PascalTypes.Integer,
            double => PascalTypes.Real,
            string => PascalTypes.Character,
            _ => throw new SemanticException("Unsupported literal.", literal.Span),
        };
    }

    private static PascalType InferIdentifierType(Identifier identifier)
    {
        if (string.Equals(identifier.Name.Lexeme, "true", StringComparison.OrdinalIgnoreCase)
            || string.Equals(identifier.Name.Lexeme, "false", StringComparison.OrdinalIgnoreCase))
        {
            return PascalTypes.Boolean;
        }

        throw new SemanticException(
            $"Undefined identifier '{identifier.Name.Lexeme}'.",
            identifier.Name.Span);
    }

    private static PascalType InferUnaryType(Unary unary)
    {
        PascalType operand = InferType(unary.Right);
        return unary.UnaryOperator.Type switch
        {
            TokenType.Plus or TokenType.Minus => RequireNumeric(
                operand,
                unary.UnaryOperator,
                "Operand must be numeric."),
            TokenType.Not => RequireBoolean(
                operand,
                unary.UnaryOperator,
                "Operand of 'not' must be Boolean."),
            _ => throw new SemanticException("Unsupported unary operator.", unary.UnaryOperator.Span),
        };
    }

    private static PascalType InferBinaryType(Binary binary)
    {
        PascalType left = InferType(binary.Left);
        PascalType right = InferType(binary.Right);
        Token binaryOperator = binary.BinaryOperator;

        return binaryOperator.Type switch
        {
            TokenType.Plus or TokenType.Minus or TokenType.Star => InferNumericResult(
                left,
                right,
                binaryOperator),
            TokenType.Slash => InferDivisionResult(left, right, binaryOperator),
            TokenType.Div or TokenType.Mod => InferIntegerResult(left, right, binaryOperator),
            TokenType.And or TokenType.Or => InferBooleanResult(left, right, binaryOperator),
            TokenType.Equal or TokenType.NotEqual or TokenType.LessThan
                or TokenType.LessThanOrEqual or TokenType.GreaterThan
                or TokenType.GreaterThanOrEqual => InferComparisonResult(left, right, binaryOperator),
            TokenType.In => throw new SemanticException(
                "Set membership is not implemented.",
                binaryOperator.Span),
            _ => throw new SemanticException("Unsupported binary operator.", binaryOperator.Span),
        };
    }

    private static PascalType InferNumericResult(
        PascalType left,
        PascalType right,
        Token binaryOperator)
    {
        RequireNumeric(left, binaryOperator, "Operands must be numeric.");
        RequireNumeric(right, binaryOperator, "Operands must be numeric.");
        return ReferenceEquals(left, PascalTypes.Real) || ReferenceEquals(right, PascalTypes.Real)
            ? PascalTypes.Real
            : PascalTypes.Integer;
    }

    private static PascalType InferDivisionResult(
        PascalType left,
        PascalType right,
        Token binaryOperator)
    {
        RequireNumeric(left, binaryOperator, "Operands must be numeric.");
        RequireNumeric(right, binaryOperator, "Operands must be numeric.");
        return PascalTypes.Real;
    }

    private static PascalType InferIntegerResult(
        PascalType left,
        PascalType right,
        Token binaryOperator)
    {
        RequireInteger(left, binaryOperator);
        RequireInteger(right, binaryOperator);
        return PascalTypes.Integer;
    }

    private static PascalType InferBooleanResult(
        PascalType left,
        PascalType right,
        Token binaryOperator)
    {
        RequireBoolean(left, binaryOperator, $"Operands of '{binaryOperator.Lexeme}' must be Boolean.");
        RequireBoolean(right, binaryOperator, $"Operands of '{binaryOperator.Lexeme}' must be Boolean.");
        return PascalTypes.Boolean;
    }

    private static PascalType InferComparisonResult(
        PascalType left,
        PascalType right,
        Token binaryOperator)
    {
        if (IsNumeric(left) && IsNumeric(right)
            || ReferenceEquals(left, right))
        {
            return PascalTypes.Boolean;
        }

        throw new SemanticException("Operands are not comparable.", binaryOperator.Span);
    }

    private static PascalType RequireNumeric(PascalType type, Token token, string message)
    {
        if (IsNumeric(type))
        {
            return type;
        }

        throw new SemanticException(message, token.Span);
    }

    private static PascalType RequireBoolean(PascalType type, Token token, string message)
    {
        if (ReferenceEquals(type, PascalTypes.Boolean))
        {
            return type;
        }

        throw new SemanticException(message, token.Span);
    }

    private static void RequireInteger(PascalType type, Token token)
    {
        if (!ReferenceEquals(type, PascalTypes.Integer))
        {
            throw new SemanticException("Operands must be integers.", token.Span);
        }
    }

    private static bool IsNumeric(PascalType type)
    {
        return ReferenceEquals(type, PascalTypes.Integer)
            || ReferenceEquals(type, PascalTypes.Real);
    }
}