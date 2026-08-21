using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Parser.Declarations;
using Cjb.StandardPascal.Language.Parser.Types;
using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics.Types;

namespace Cjb.StandardPascal.Language.Semantics;

public sealed class SemanticAnalyzer : ISemanticAnalyzer
{
    private readonly Dictionary<string, PascalType> _symbols = new(StringComparer.OrdinalIgnoreCase);

    public void Analyze(Program program)
    {
        ArgumentNullException.ThrowIfNull(program);

        _symbols.Clear();

        if (program.Block is not null)
        {
            foreach (Declaration declaration in program.Block.Declarations)
            {
                switch (declaration)
                {
                    case VariableDeclaration variable:
                        foreach (Scanner.Token name in variable.Names)
                        {
                            _symbols[name.Lexeme] = variable.Type is ScalarTypeSyntax scalar
                                ? scalar.Type
                                : PascalTypes.Integer;
                        }
                        break;
                    case ConstantDeclaration constant:
                        _symbols[constant.Name.Lexeme] = InferType(constant.Value);
                        break;
                    case EnumerationDeclaration enumeration:
                        foreach (Scanner.Token member in enumeration.Members) { _symbols[member.Lexeme] = PascalTypes.Integer; }
                        break;
                }
            }
        }

        AnalyzeStatement(program.Body);
    }

    private static void AnalyzeStatement(IStatement statement)
    {
        switch (statement)
        {
            case Assignment assignment:
                InferType(assignment.Value);
                return;
            case Case caseStatement:
                PascalType selectorType = InferType(caseStatement.Selector);
                foreach (CaseBranch branch in caseStatement.Branches)
                {
                    foreach (Expression label in branch.Labels)
                    {
                        PascalType labelType = InferType(label);
                        if (!ReferenceEquals(selectorType, labelType) && !(IsNumeric(selectorType) && IsNumeric(labelType)))
                        {
                            throw new SemanticException("Case label is not compatible with selector.", label.Span);
                        }
                    }

                    AnalyzeStatement(branch.Statement);
                }

                if (caseStatement.ElseBranch is not null) { AnalyzeStatement(caseStatement.ElseBranch); }
                return;
            case For forStatement:
                RequireInteger(InferType(forStatement.Initial), forStatement.Variable);
                RequireInteger(InferType(forStatement.Limit), forStatement.Variable);
                AnalyzeStatement(forStatement.Body);
                return;
            case Goto:
                return;
            case If ifStatement:
                RequireBoolean(InferType(ifStatement.Condition), new Scanner.Token(Scanner.TokenType.If, "if", null, ifStatement.Condition.Span), "Condition must be Boolean.");
                AnalyzeStatement(ifStatement.ThenBranch);
                if (ifStatement.ElseBranch is not null) { AnalyzeStatement(ifStatement.ElseBranch); }
                return;
            case Labeled labeled:
                AnalyzeStatement(labeled.Statement);
                return;
            case BlockStatement blockStatement:
                foreach (IStatement nestedStatement in blockStatement.Block.Statements)
                {
                    AnalyzeStatement(nestedStatement);
                }

                return;
            case Print print:
                InferType(print.Expression);
                return;
            case Write write:
                foreach (Expression expression in write.Expressions)
                {
                    InferType(expression);
                }

                return;
            case While whileStatement:
                RequireBoolean(InferType(whileStatement.Condition), new Scanner.Token(Scanner.TokenType.While, "while", null, whileStatement.Condition.Span), "Condition must be Boolean.");
                AnalyzeStatement(whileStatement.Body);
                return;
            case Repeat repeatStatement:
                foreach (IStatement nestedStatement in repeatStatement.Body) { AnalyzeStatement(nestedStatement); }
                RequireBoolean(InferType(repeatStatement.Condition), new Scanner.Token(Scanner.TokenType.Until, "until", null, repeatStatement.Condition.Span), "Condition must be Boolean.");
                return;
            case With withStatement:
                AnalyzeStatement(withStatement.Body);
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
            Call call => InferCallType(call),
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

    private static PascalType InferCallType(Call call)
    {
        foreach (Expression argument in call.Arguments)
        {
            InferType(argument);
        }

        return call.Name.Lexeme.Equals("chr", StringComparison.OrdinalIgnoreCase)
            ? PascalTypes.Character
            : PascalTypes.Integer;
    }

    private static PascalType InferIdentifierType(Identifier identifier)
    {
        if (string.Equals(identifier.Name.Lexeme, "true", StringComparison.OrdinalIgnoreCase)
            || string.Equals(identifier.Name.Lexeme, "false", StringComparison.OrdinalIgnoreCase))
        {
            return PascalTypes.Boolean;
        }

        return PascalTypes.Integer;
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