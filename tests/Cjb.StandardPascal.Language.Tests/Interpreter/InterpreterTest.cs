using Cjb.StandardPascal.Language.Interpreter;
using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Tests.Interpreter;

[TestClass]
public sealed class InterpreterTest
{
    private readonly IScanner _scanner = new Language.Scanner.Scanner();
    private readonly IParser _parser = new Language.Parser.Parser();
    private readonly IInterpreter _interpreter = new Language.Interpreter.Interpreter();

    [TestMethod]
    [DataRow("3 * 5", 15L)]
    [DataRow("7 div 2", 3L)]
    [DataRow("7 mod 2", 1L)]
    [DataRow("1 + 2 * 3", 7L)]
    [DataRow("-5 + 2", -3L)]
    public void Evaluate_Integer_Expression_Returns_Integer(
        string source,
        long expected)
    {
        object result = Evaluate(source);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("5 / 2", 2.5)]
    [DataRow("1 + 2.5", 3.5)]
    [DataRow("-(2.5)", -2.5)]
    public void Evaluate_Real_Expression_Returns_Real(
        string source,
        double expected)
    {
        object result = Evaluate(source);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("true and not false", true)]
    [DataRow("false or false", false)]
    [DataRow("3 < 4", true)]
    [DataRow("3.0 = 3", true)]
    [DataRow("true > false", true)]
    public void Evaluate_Boolean_Expression_Returns_Boolean(
        string source,
        bool expected)
    {
        object result = Evaluate(source);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Interpret_Print_Returns_Expression_Value()
    {
        List<Token> tokens = _scanner.ScanTokens(new SourceText("Print 3 * 5;"));
        IStatement statement = _parser.ParseStatement(tokens);

        object result = _interpreter.Interpret(statement);

        Assert.AreEqual(15L, result);
    }

    [TestMethod]
    public void Interpret_Print_String_Returns_String_Value()
    {
        List<Token> tokens = _scanner.ScanTokens(
            new SourceText("Print 'Hello, world!';"));
        IStatement statement = _parser.ParseStatement(tokens);

        object result = _interpreter.Interpret(statement);

        Assert.AreEqual("Hello, world!", result);
    }

    [TestMethod]
    [DataRow("'alpha' = 'alpha'", true)]
    [DataRow("'alpha' < 'beta'", true)]
    [DataRow("'beta' <> 'beta'", false)]
    public void Evaluate_String_Comparison_Returns_Boolean(
        string source,
        bool expected)
    {
        object result = Evaluate(source);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("1 / 0", "Division by zero.")]
    [DataRow("1 div 0", "Division by zero.")]
    [DataRow("1 and 2", "Operand must be Boolean.")]
    [DataRow("1.5 div 1", "Operands must be integers.")]
    [DataRow("unknown + 1", "Undefined identifier 'unknown'.")]
    public void Evaluate_Invalid_Expression_Throws_Runtime_Exception(
        string source,
        string message)
    {
        RuntimeException exception = Assert.ThrowsExactly<RuntimeException>(
            () => Evaluate(source));

        Assert.AreEqual(message, exception.Message);
    }

    [TestMethod]
    public void Evaluate_Integer_Overflow_Throws_Runtime_Exception()
    {
        RuntimeException exception = Assert.ThrowsExactly<RuntimeException>(
            () => Evaluate("9223372036854775807 + 1"));

        Assert.AreEqual("Integer arithmetic overflow.", exception.Message);
    }

    private object Evaluate(string source)
    {
        List<Token> tokens = _scanner.ScanTokens(new SourceText(source));
        Expression expression = _parser.Parse(tokens);
        return _interpreter.Evaluate(expression);
    }
}