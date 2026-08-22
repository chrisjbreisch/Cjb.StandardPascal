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
    private static readonly PascalType StructuredType = new PrimitivePascalType("structured");
    private readonly Dictionary<string, PascalType> _symbols = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _activeForControls = new(StringComparer.OrdinalIgnoreCase);
    private readonly Stack<HashSet<long>> _blockLabels = [];

    public void Analyze(Program program)
    {
        ArgumentNullException.ThrowIfNull(program);

        _symbols.Clear();
        _activeForControls.Clear();
        _blockLabels.Clear();

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
                                : StructuredType;
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

    private void AnalyzeStatement(IStatement statement)
    {
        switch (statement)
        {
            case Assignment assignment:
                if (_activeForControls.Contains(assignment.Name.Lexeme))
                {
                    throw new SemanticException(
                        $"Cannot assign to active for control variable '{assignment.Name.Lexeme}'.",
                        assignment.Name.Span);
                }

                PascalType sourceType = InferType(assignment.Value);
                if (_symbols.TryGetValue(assignment.Name.Lexeme, out PascalType? targetType)
                    && !ReferenceEquals(targetType, StructuredType)
                    && !targetType.IsAssignmentCompatibleWith(sourceType))
                {
                    throw new SemanticException(
                        $"Cannot assign {sourceType.Name} to {targetType.Name} '{assignment.Name.Lexeme}'.",
                        assignment.Name.Span);
                }

                if (_symbols.TryGetValue(assignment.Name.Lexeme, out targetType)
                    && ReferenceEquals(targetType, PascalTypes.Character)
                    && assignment.Value is Literal { Value: string text }
                    && text.Length != 1)
                {
                    throw new SemanticException(
                        "Character value must contain exactly one character.",
                        assignment.Value.Span);
                }

                return;
            case IndexedAssignment indexedAssignment:
                foreach (Expression subscript in indexedAssignment.Subscripts) { RequireInteger(InferType(subscript), indexedAssignment.Name); }
                InferType(indexedAssignment.Value);
                return;
            case Allocation:
                return;
            case DereferenceAssignment dereferenceAssignment:
                InferType(dereferenceAssignment.Value);
                return;
            case FieldAssignment fieldAssignment:
                InferType(fieldAssignment.Value);
                return;
            case Case caseStatement:
                PascalType selectorType = InferType(caseStatement.Selector);
                foreach (CaseBranch branch in caseStatement.Branches)
                {
                    foreach (Expression label in branch.Labels)
                    {
                        PascalType labelType = InferType(label);
                        if (!ReferenceEquals(selectorType, StructuredType)
                            && !ReferenceEquals(selectorType, labelType)
                            && !(IsNumeric(selectorType) && IsNumeric(labelType)))
                        {
                            throw new SemanticException("Case label is not compatible with selector.", label.Span);
                        }
                    }

                    AnalyzeStatement(branch.Statement);
                }

                if (caseStatement.ElseBranch is not null) { AnalyzeStatement(caseStatement.ElseBranch); }
                return;
            case For forStatement:
                RequireOrdinal(InferType(forStatement.Initial), forStatement.Variable);
                RequireOrdinal(InferType(forStatement.Limit), forStatement.Variable);
                _activeForControls.Add(forStatement.Variable.Lexeme);
                try { AnalyzeStatement(forStatement.Body); }
                finally { _activeForControls.Remove(forStatement.Variable.Lexeme); }
                return;
            case Goto gotoStatement:
                if (_blockLabels.Count == 0 || !_blockLabels.Peek().Contains((long)gotoStatement.Label.Literal!))
                {
                    throw new SemanticException(
                        $"Goto target '{gotoStatement.Label.Lexeme}' is not declared in this block.",
                        gotoStatement.Label.Span);
                }

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
                HashSet<long> labels = blockStatement.Block.Statements
                    .OfType<Labeled>()
                    .Select(static labeled => (long)labeled.Label.Literal!)
                    .ToHashSet();
                _blockLabels.Push(labels);

                try
                {
                    foreach (IStatement nestedStatement in blockStatement.Block.Statements)
                    {
                
                        AnalyzeStatement(nestedStatement);
                    }
                }
                finally { _blockLabels.Pop(); }

                return;
            case Print print:
                InferType(print.Expression);
                return;
            case ProcedureCall:
                return;
            case Read:
                return;
            case Write write:
                foreach (WriteItem item in write.Items)
                {
                    InferType(item.Expression);
                    if (item.Width is not null) { RequireInteger(InferType(item.Width), new Scanner.Token(Scanner.TokenType.Colon, ":", null, item.Width.Span)); }
                    if (item.Precision is not null) { RequireInteger(InferType(item.Precision), new Scanner.Token(Scanner.TokenType.Colon, ":", null, item.Precision.Span)); }
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
                // Record fields are resolved dynamically by the active with scope.
                return;
            default:
                throw new SemanticException("Unsupported statement.", statement.Span);
        }
    }

    private PascalType InferType(Expression expression)
    {
        return expression switch
        {
            Literal literal => InferLiteralType(literal),
            Nil => new PointerPascalType("pointer"),
            Call call => InferCallType(call),
            SetLiteral setLiteral => InferSetLiteralType(setLiteral),
            SetRange => new PrimitivePascalType("set"),
            Identifier identifier => InferIdentifierType(identifier),
            Cjb.StandardPascal.Language.Parser.Expressions.Index index => InferIndexType(index),
            Field => PascalTypes.Integer,
            Dereference => PascalTypes.Integer,
            Grouping grouping => InferType(grouping.InnerExpression),
            Unary unary => InferUnaryType(unary),
            Binary binary => InferBinaryType(binary),
            _ => throw new SemanticException("Unsupported expression.", expression.Span),
        };
    }

    private PascalType InferLiteralType(Literal literal)
    {
        return literal.Value switch
        {
            long => PascalTypes.Integer,
            double => PascalTypes.Real,
            string => PascalTypes.Character,
            _ => throw new SemanticException("Unsupported literal.", literal.Span),
        };
    }

    private PascalType InferIndexType(Cjb.StandardPascal.Language.Parser.Expressions.Index index)
    {
        foreach (Expression subscript in index.Subscripts) { RequireInteger(InferType(subscript), index.Name); }
        return PascalTypes.Integer;
    }

    private PascalType InferCallType(Call call)
    {
        foreach (Expression argument in call.Arguments)
        {
            InferType(argument);
        }

        return call.Name.Lexeme.Equals("chr", StringComparison.OrdinalIgnoreCase)
            ? PascalTypes.Character
            : PascalTypes.Integer;
    }

    private PascalType InferSetLiteralType(SetLiteral setLiteral)
    {
        foreach (Expression element in setLiteral.Elements)
        {
            if (element is SetRange range)
            {
                RequireInteger(InferType(range.Lower), new Scanner.Token(Scanner.TokenType.Range, "..", null, range.Lower.Span));
                RequireInteger(InferType(range.Upper), new Scanner.Token(Scanner.TokenType.Range, "..", null, range.Upper.Span));
            }
            else
            {
                RequireInteger(InferType(element), new Scanner.Token(Scanner.TokenType.In, "in", null, element.Span));
            }
        }

        return new PrimitivePascalType("set");
    }

    private PascalType InferIdentifierType(Identifier identifier)
    {
        if (string.Equals(identifier.Name.Lexeme, "true", StringComparison.OrdinalIgnoreCase)
            || string.Equals(identifier.Name.Lexeme, "false", StringComparison.OrdinalIgnoreCase))
        {
            return PascalTypes.Boolean;
        }

        return _symbols.TryGetValue(identifier.Name.Lexeme, out PascalType? type)
            ? type
            : throw new SemanticException(
                $"Undefined identifier '{identifier.Name.Lexeme}'.",
                identifier.Name.Span);
    }

    private PascalType InferUnaryType(Unary unary)
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

    private PascalType InferBinaryType(Binary binary)
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
            TokenType.In => InferMembershipResult(left, right, binaryOperator),
            _ => throw new SemanticException("Unsupported binary operator.", binaryOperator.Span),
        };
    }

    private PascalType InferNumericResult(
        PascalType left,
        PascalType right,
        Token binaryOperator)
    {
        if (ReferenceEquals(left, PascalTypes.Character)
            && ReferenceEquals(right, PascalTypes.Character))
        {
            return PascalTypes.Character;
        }

        if (string.Equals(left.Name, "set", StringComparison.Ordinal)
            && string.Equals(right.Name, "set", StringComparison.Ordinal))
        {
            return new PrimitivePascalType("set");
        }

        RequireNumeric(left, binaryOperator, "Operands must be numeric.");
        RequireNumeric(right, binaryOperator, "Operands must be numeric.");
        return ReferenceEquals(left, PascalTypes.Real) || ReferenceEquals(right, PascalTypes.Real)
            ? PascalTypes.Real
            : PascalTypes.Integer;
    }

    private PascalType InferDivisionResult(
        PascalType left,
        PascalType right,
        Token binaryOperator)
    {
        RequireNumeric(left, binaryOperator, "Operands must be numeric.");
        RequireNumeric(right, binaryOperator, "Operands must be numeric.");
        return PascalTypes.Real;
    }

    private PascalType InferIntegerResult(
        PascalType left,
        PascalType right,
        Token binaryOperator)
    {
        RequireInteger(left, binaryOperator);
        RequireInteger(right, binaryOperator);
        return PascalTypes.Integer;
    }

    private PascalType InferBooleanResult(
        PascalType left,
        PascalType right,
        Token binaryOperator)
    {
        RequireBoolean(left, binaryOperator, $"Operands of '{binaryOperator.Lexeme}' must be Boolean.");
        RequireBoolean(right, binaryOperator, $"Operands of '{binaryOperator.Lexeme}' must be Boolean.");
        return PascalTypes.Boolean;
    }

    private PascalType InferComparisonResult(
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

    private PascalType InferMembershipResult(PascalType left, PascalType right, Token token)
    {
        RequireInteger(left, token);
        return PascalTypes.Boolean;
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

    private static void RequireOrdinal(PascalType type, Token token)
    {
        if (!ReferenceEquals(type, PascalTypes.Integer)
            && !ReferenceEquals(type, PascalTypes.Character))
        {
            throw new SemanticException("For bounds must be ordinal.", token.Span);
        }
    }

    private static bool IsNumeric(PascalType type)
    {
        return ReferenceEquals(type, PascalTypes.Integer)
            || ReferenceEquals(type, PascalTypes.Real);
    }
}