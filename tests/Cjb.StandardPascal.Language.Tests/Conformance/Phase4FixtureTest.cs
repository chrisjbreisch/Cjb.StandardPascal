using Cjb.StandardPascal.Language.Interpreter;
using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Tests.Conformance;

[TestClass]
public sealed class Phase4FixtureTest
{
    [TestMethod]
    public void CompositePositive_Fixture_Executes()
    {
        string source = File.ReadAllText(FixturePath("CompositePositive.pas"));
        BufferOutput output = new();
        Program program = Parse(source, "CompositePositive.pas");

        new Language.Interpreter.Interpreter(output).Execute(program);

        Assert.AreEqual("3" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void PointerAfterDisposeNegative_Fixture_Reports_Runtime_Error()
    {
        string source = File.ReadAllText(FixturePath("PointerAfterDisposeNegative.pas"));
        Program program = Parse(source, "PointerAfterDisposeNegative.pas");

        RuntimeException exception = Assert.ThrowsExactly<RuntimeException>(
            () => new Language.Interpreter.Interpreter().Execute(program));

        Assert.AreEqual("Pointer is nil or disposed.", exception.Message);
    }

    private static Program Parse(string source, string filePath)
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        return parser.ParseProgram(scanner.ScanTokens(new SourceText(source, filePath)));
    }

    private static string FixturePath(string fileName)
    {
        return Path.Combine(
            AppContext.BaseDirectory,
            "Fixtures",
            "Phase4",
            fileName);
    }

    private sealed class BufferOutput : IOutput
    {
        public string Text { get; private set; } = string.Empty;

        public void Write(string value) => Text += value;

        public void WriteLine(string value) => Text += value + Environment.NewLine;
    }
}