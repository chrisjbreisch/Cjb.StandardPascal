using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Parser.Declarations;
using Cjb.StandardPascal.Language.Parser.Types;
using Cjb.StandardPascal.Language.Parser.Routines;
using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics;
using Cjb.StandardPascal.Language.Semantics.Types;
using System.Globalization;

namespace Cjb.StandardPascal.Language.Interpreter;

public sealed class Interpreter : IInterpreter
{
    private readonly ISemanticAnalyzer _semanticAnalyzer;
    private readonly IOutput _output;
    private readonly Dictionary<string, object> _values = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, PascalType> _types = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, PascalType> _namedTypes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, ProcedureDeclaration> _procedures = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, FunctionDeclaration> _functions = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, object>? _activeFields;

    public Interpreter()
        : this(new SemanticAnalyzer(), new NullOutput())
    {
    }

    public Interpreter(IOutput output)
        : this(new SemanticAnalyzer(), output)
    {
    }

    public Interpreter(ISemanticAnalyzer semanticAnalyzer)
        : this(semanticAnalyzer, new NullOutput())
    {
    }

    public Interpreter(ISemanticAnalyzer semanticAnalyzer, IOutput output)
    {
        _semanticAnalyzer = semanticAnalyzer
            ?? throw new ArgumentNullException(nameof(semanticAnalyzer));
        _output = output ?? throw new ArgumentNullException(nameof(output));
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
        _values.Clear();
        _types.Clear();
        _namedTypes.Clear();
        _procedures.Clear();
        _functions.Clear();
        _activeFields = null;

        if (program.Block is not null)
        {
            foreach (Declaration declaration in program.Block.Declarations)
            {
                switch (declaration)
                {
                    case EnumerationDeclaration enumeration:
                        PascalType enumType = new PrimitivePascalType(enumeration.Name.Lexeme);
                        _namedTypes.Add(enumeration.Name.Lexeme, enumType);
                        foreach (Token member in enumeration.Members)
                        {
                            _values.Add(member.Lexeme, member.Lexeme);
                            _types.Add(member.Lexeme, enumType);
                        }
                        break;
                    case SubrangeDeclaration subrange:
                        _namedTypes.Add(subrange.Name.Lexeme, new SubrangePascalType(subrange.Name.Lexeme, subrange.Minimum, subrange.Maximum));
                        break;
                    case RecordDeclaration record:
                        _namedTypes.Add(record.Name.Lexeme, new PrimitivePascalType(record.Name.Lexeme));
                        break;
                    case ProcedureDeclaration procedure:
                        _procedures.Add(procedure.Name.Lexeme, procedure);
                        break;
                    case FunctionDeclaration function:
                        _functions.Add(function.Name.Lexeme, function);
                        break;
                    case ConstantDeclaration constant:
                        _values.Add(constant.Name.Lexeme, Evaluate(constant.Value));
                        _types.Add(constant.Name.Lexeme, TypeOf(_values[constant.Name.Lexeme]));
                        break;
                    case VariableDeclaration variable:
                        foreach (Token name in variable.Names)
                        {
                            PascalType variableType = ResolveType(variable.Type);
                            _values.Add(name.Lexeme, program.Block.Declarations.OfType<RecordDeclaration>().FirstOrDefault(record => string.Equals(record.Name.Lexeme, variableType.Name, StringComparison.OrdinalIgnoreCase)) is RecordDeclaration record
                                ? record.Fields.ToDictionary(static field => field.Lexeme, static _ => (object)0L, StringComparer.OrdinalIgnoreCase)
                                : DefaultValue(variableType));
                            _types.Add(name.Lexeme, variableType);
                        }

                        break;
                }
            }
        }

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

    public object VisitCallExpression(Call expression)
    {
        if (_functions.TryGetValue(expression.Name.Lexeme, out FunctionDeclaration? function))
        {
            return InvokeFunction(function, expression);
        }

        if (expression.Arguments.Count != 1)
        {
            throw Error(expression.Name, $"Routine '{expression.Name.Lexeme}' expects one argument.");
        }

        object argument = Evaluate(expression.Arguments[0]);
        return expression.Name.Lexeme.ToLowerInvariant() switch
        {
            "ord" when argument is string { Length: 1 } character => (long)character[0],
            "chr" => char.ConvertFromUtf32(checked((int)RequireInteger(expression.Name, argument))),
            "succ" => Successor(expression.Name, argument, 1),
            "pred" => Successor(expression.Name, argument, -1),
            "round" => checked((long)Math.Round(ToDouble(expression.Name, argument), MidpointRounding.AwayFromZero)),
            "trunc" => checked((long)Math.Truncate(ToDouble(expression.Name, argument))),
            _ => throw Error(expression.Name, $"Unsupported routine '{expression.Name.Lexeme}'."),
        };
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

        if (_values.TryGetValue(expression.Name.Lexeme, out object? value))
        {
            return value;
        }

        if (_activeFields is not null && _activeFields.TryGetValue(expression.Name.Lexeme, out object? fieldValue))
        {
            return fieldValue;
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

    public object VisitProcedureCallStatement(ProcedureCall statement)
    {
        if (!_procedures.TryGetValue(statement.Name.Lexeme, out ProcedureDeclaration? procedure))
        {
            throw Error(statement.Name, $"Undefined procedure '{statement.Name.Lexeme}'.");
        }

        if (statement.Arguments.Count != procedure.Parameters.Count)
        {
            throw Error(statement.Name, $"Procedure '{procedure.Name.Lexeme}' expects {procedure.Parameters.Count} arguments.");
        }

        object[] arguments = statement.Arguments.Select(Evaluate).ToArray();
        Dictionary<string, object> callerValues = new(_values, StringComparer.OrdinalIgnoreCase);
        Dictionary<string, PascalType> callerTypes = new(_types, StringComparer.OrdinalIgnoreCase);
        Dictionary<string, string> variableArguments = [];

        try
        {
            for (int index = 0; index < procedure.Parameters.Count; index++)
            {
                RoutineParameter parameter = procedure.Parameters[index];
                PascalType type = ResolveType(parameter.Type);
                _values[parameter.Name.Lexeme] = arguments[index];
                _types[parameter.Name.Lexeme] = type;

                if (parameter.IsVariable)
                {
                    if (statement.Arguments[index] is not Identifier identifier)
                    {
                        throw Error(statement.Name, "Var parameter requires an assignable identifier.");
                    }

                    variableArguments.Add(parameter.Name.Lexeme, identifier.Name.Lexeme);
                }
            }

            InitializeRoutineBlock(procedure.Body);
            object result = Interpret(new BlockStatement(procedure.Body));

            foreach ((string parameterName, string callerName) in variableArguments)
            {
                callerValues[callerName] = _values[parameterName];
            }

            return result;
        }
        finally
        {
            _values.Clear();
            _types.Clear();
            foreach ((string name, object value) in callerValues) { _values.Add(name, value); }
            foreach ((string name, PascalType type) in callerTypes) { _types.Add(name, type); }
        }
    }

    public object VisitBlockStatement(BlockStatement statement)
    {
        object result = string.Empty;
        Dictionary<long, int> labels = statement.Block.Statements
            .Select((nestedStatement, index) => (nestedStatement, index))
            .Where(static item => item.nestedStatement is Labeled)
            .ToDictionary(
                static item => (long)((Labeled)item.nestedStatement).Label.Literal!,
                static item => item.index);

        for (int index = 0; index < statement.Block.Statements.Count; index++)
        {
            try { result = Interpret(statement.Block.Statements[index]); }
            catch (GotoSignal signal) when (labels.TryGetValue(signal.Label, out int target)) { index = target - 1; }
        }

        return result;
    }

    public object VisitGotoStatement(Goto statement) => throw new GotoSignal((long)statement.Label.Literal!);

    public object VisitLabeledStatement(Labeled statement) => Interpret(statement.Statement);

    public object VisitAssignmentStatement(Assignment statement)
    {
        if (_activeFields is not null && _activeFields.ContainsKey(statement.Name.Lexeme))
        {
            object fieldValue = Evaluate(statement.Value);
            _activeFields[statement.Name.Lexeme] = fieldValue;
            return fieldValue;
        }

        if (!_types.TryGetValue(statement.Name.Lexeme, out PascalType? type))
        {
            throw Error(statement.Name, $"Undefined identifier '{statement.Name.Lexeme}'.");
        }

        object value = Evaluate(statement.Value);
        PascalType sourceType = statement.Value is Identifier identifier
            && _types.TryGetValue(identifier.Name.Lexeme, out PascalType? identifierType)
            ? identifierType
            : TypeOf(value);

        if (!type.IsAssignmentCompatibleWith(sourceType))
        {
            throw Error(statement.Name, $"Cannot assign {sourceType.Name} to {type.Name} '{statement.Name.Lexeme}'.");
        }

        if (type is SubrangePascalType subrange && value is long subrangeValue && (subrangeValue < subrange.Minimum || subrangeValue > subrange.Maximum))
        {
            throw Error(statement.Name, $"Value {subrangeValue} is outside subrange {subrange.Minimum}..{subrange.Maximum}.");
        }

        _values[statement.Name.Lexeme] = ReferenceEquals(type, PascalTypes.Real) && value is long integer
            ? (double)integer
            : value;
        return _values[statement.Name.Lexeme];
    }

    public object VisitCaseStatement(Case statement)
    {
        object selector = Evaluate(statement.Selector);

        foreach (CaseBranch branch in statement.Branches)
        {
            if (branch.Labels.Any(label => Equals(selector, Evaluate(label))))
            {
                return Interpret(branch.Statement);
            }
        }

        return statement.ElseBranch is null ? string.Empty : Interpret(statement.ElseBranch);
    }

    public object VisitWithStatement(With statement)
    {
        if (!_values.TryGetValue(statement.Record.Lexeme, out object? value)
            || value is not Dictionary<string, object> fields)
        {
            throw Error(statement.Record, $"'{statement.Record.Lexeme}' is not a record variable.");
        }

        Dictionary<string, object>? previousFields = _activeFields;
        _activeFields = fields;

        try
        {
            return Interpret(statement.Body);
        }
        finally
        {
            _activeFields = previousFields;
        }
    }

    public object VisitIfStatement(If statement)
    {
        return RequireBoolean(statement.Condition.Span, Evaluate(statement.Condition))
            ? Interpret(statement.ThenBranch)
            : statement.ElseBranch is null ? string.Empty : Interpret(statement.ElseBranch);
    }

    public object VisitWhileStatement(While statement)
    {
        object result = string.Empty;

        while (RequireBoolean(statement.Condition.Span, Evaluate(statement.Condition)))
        {
            result = Interpret(statement.Body);
        }

        return result;
    }

    public object VisitRepeatStatement(Repeat statement)
    {
        object result;

        do
        {
            result = string.Empty;

            foreach (IStatement nestedStatement in statement.Body)
            {
                result = Interpret(nestedStatement);
            }
        }
        while (!RequireBoolean(statement.Condition.Span, Evaluate(statement.Condition)));

        return result;
    }

    public object VisitForStatement(For statement)
    {
        if (!_types.TryGetValue(statement.Variable.Lexeme, out PascalType? type)
            || !ReferenceEquals(type, PascalTypes.Integer))
        {
            throw new RuntimeException("For control variable must be an integer variable.", statement.Variable.Span);
        }

        long initial = RequireInteger(statement.Variable, Evaluate(statement.Initial));
        long limit = RequireInteger(statement.Variable, Evaluate(statement.Limit));
        _values[statement.Variable.Lexeme] = initial;
        object result = string.Empty;

        for (long value = initial;
            statement.Direction == ForDirection.To ? value <= limit : value >= limit;
            value = checked(value + (statement.Direction == ForDirection.To ? 1 : -1)))
        {
            _values[statement.Variable.Lexeme] = value;
            result = Interpret(statement.Body);
        }

        return result;
    }

    public object VisitWriteStatement(Write statement)
    {
        string value = string.Concat(statement.Expressions.Select(expression => FormatValue(Evaluate(expression))));

        if (statement.AppendNewLine)
        {
            _output.WriteLine(value);
        }
        else
        {
            _output.Write(value);
        }

        return value;
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

    private static bool RequireBoolean(SourceSpan span, object value)
    {
        return value is bool boolean
            ? boolean
            : throw new RuntimeException("Condition must be Boolean.", span);
    }

    private static long RequireInteger(Token token, object value)
    {
        return value is long integer
            ? integer
            : throw Error(token, "Operands must be integers.");
    }

    private static object Successor(Token token, object value, int delta)
    {
        return value switch
        {
            long integer => checked(integer + delta),
            string { Length: 1 } character => char.ConvertFromUtf32(checked(character[0] + delta)),
            _ => throw Error(token, "Operand must be ordinal."),
        };
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

    private static PascalType TypeOf(object value) => value switch
    {
        long => PascalTypes.Integer,
        double => PascalTypes.Real,
        bool => PascalTypes.Boolean,
        string => PascalTypes.Character,
        _ => throw new InvalidOperationException("Unsupported runtime value."),
    };

    private object InvokeFunction(FunctionDeclaration function, Call call)
    {
        if (call.Arguments.Count != function.Parameters.Count)
        {
            throw Error(call.Name, $"Function '{function.Name.Lexeme}' expects {function.Parameters.Count} arguments.");
        }

        object[] arguments = call.Arguments.Select(Evaluate).ToArray();
        Dictionary<string, object> callerValues = new(_values, StringComparer.OrdinalIgnoreCase);
        Dictionary<string, PascalType> callerTypes = new(_types, StringComparer.OrdinalIgnoreCase);

        try
        {
            _values.Clear();
            _types.Clear();

            for (int index = 0; index < function.Parameters.Count; index++)
            {
                RoutineParameter parameter = function.Parameters[index];
                PascalType type = ResolveType(parameter.Type);
                _values.Add(parameter.Name.Lexeme, arguments[index]);
                _types.Add(parameter.Name.Lexeme, type);
            }

            PascalType returnType = ResolveType(function.ReturnType);
            _values.Add(function.Name.Lexeme, DefaultValue(returnType));
            _types.Add(function.Name.Lexeme, returnType);
            Interpret(new BlockStatement(function.Body));
            return _values[function.Name.Lexeme];
        }
        finally
        {
            _values.Clear();
            _types.Clear();
            foreach ((string name, object value) in callerValues) { _values.Add(name, value); }
            foreach ((string name, PascalType type) in callerTypes) { _types.Add(name, type); }
        }
    }

    private void InitializeRoutineBlock(Block block)
    {
        foreach (Declaration declaration in block.Declarations)
        {
            switch (declaration)
            {
                case VariableDeclaration variable:
                    foreach (Token name in variable.Names)
                    {
                        PascalType type = ResolveType(variable.Type);
                        _values[name.Lexeme] = DefaultValue(type);
                        _types[name.Lexeme] = type;
                    }
                    break;
                case ProcedureDeclaration procedure:
                    _procedures[procedure.Name.Lexeme] = procedure;
                    break;
                case FunctionDeclaration function:
                    _functions[function.Name.Lexeme] = function;
                    break;
            }
        }
    }

    private PascalType ResolveType(TypeSyntax type) => type switch
    {
        ScalarTypeSyntax scalar => scalar.Type,
        NamedTypeSyntax named when _namedTypes.TryGetValue(named.Name.Lexeme, out PascalType? resolved) => resolved,
        NamedTypeSyntax named => throw new RuntimeException($"Undefined type '{named.Name.Lexeme}'.", named.Span),
        _ => throw new RuntimeException("Unsupported type.", type.Span),
    };

    private static object DefaultValue(PascalType type) => ReferenceEquals(type, PascalTypes.Integer) ? 0L
        : ReferenceEquals(type, PascalTypes.Real) ? 0d
        : ReferenceEquals(type, PascalTypes.Boolean) ? false
        : string.Empty;

    private static string FormatValue(object value) => value switch
    {
        bool boolean => boolean ? "TRUE" : "FALSE",
        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty,
    };

    private sealed class GotoSignal : Exception
    {
        public GotoSignal(long label) { Label = label; }
        public long Label { get; }
    }
}