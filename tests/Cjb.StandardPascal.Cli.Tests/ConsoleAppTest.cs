using Cjb.StandardPascal.Application;
using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

using Microsoft.Extensions.Logging.Abstractions;

namespace Cjb.StandardPascal.Cli.Tests;

[TestClass]
public sealed class ConsoleAppTest
{
    [TestMethod]
    public void Run_Expression_Prints_Scanned_Tokens()
    {
        TestConsole console = new(["1 + 2", ""]);
        ConsoleApp application = new(
            NullLogger<ConsoleApp>.Instance,
            new Scanner(),
            new Parser(),
            new ExpressionFormatter(),
            console);

        int exitCode = application.Run([]);

        Assert.AreEqual(0, exitCode);
        Assert.Contains("Number 1 1", console.Output);
        Assert.Contains("Plus + ", console.Output);
        Assert.Contains("Number 2 2", console.Output);
        Assert.Contains("EndOfFile  ", console.Output);
        Assert.Contains("Parse:", console.Output);
        Assert.Contains("(+ 1 2)", console.Output);
    }

    [TestMethod]
    public void Run_Invalid_Expression_Prints_Error_And_Continues()
    {
        TestConsole console = new(["1 @ 2", "3", ""]);
        ConsoleApp application = new(
            NullLogger<ConsoleApp>.Instance,
            new Scanner(),
            new Parser(),
            new ExpressionFormatter(),
            console);

        int exitCode = application.Run([]);

        Assert.AreEqual(0, exitCode);
        Assert.Contains("error (1,3): Unexpected character '@'.", console.Output);
        Assert.Contains("Number 3 3", console.Output);
        Assert.Contains("Parse:", console.Output);
        Assert.Contains("  3", console.Output);
    }

    [TestMethod]
    public void Run_Invalid_Syntax_Prints_Error_And_Continues()
    {
        TestConsole console = new(["1 +", "2 * 3", ""]);
        ConsoleApp application = new(
            NullLogger<ConsoleApp>.Instance,
            new Scanner(),
            new Parser(),
            new ExpressionFormatter(),
            console);

        int exitCode = application.Run([]);

        Assert.AreEqual(0, exitCode);
        Assert.Contains("error (1,4): Expected an expression.", console.Output);
        Assert.Contains("(* 2 3)", console.Output);
    }

    [TestMethod]
    public void Run_EndOfInput_Exits_Successfully()
    {
        TestConsole console = new([]);
        ConsoleApp application = new(
            NullLogger<ConsoleApp>.Instance,
            new Scanner(),
            new Parser(),
            new ExpressionFormatter(),
            console);

        int exitCode = application.Run([]);

        Assert.AreEqual(0, exitCode);
    }

    private sealed class TestConsole : IConsole
    {
        private readonly Queue<string> _input;
        private readonly StringWriter _output = new();

        public TestConsole(IEnumerable<string> input)
        {
            _input = new Queue<string>(input);
        }

        public string Output => _output.ToString();

        public string? ReadLine()
        {
            return _input.TryDequeue(out string? value) ? value : null;
        }

        public void Write(string value)
        {
            _output.Write(value);
        }

        public void WriteLine(string value)
        {
            _output.WriteLine(value);
        }
    }
}