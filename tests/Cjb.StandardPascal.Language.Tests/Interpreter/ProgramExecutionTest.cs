using Cjb.StandardPascal.Language.Interpreter;
using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Tests.Interpreter;

[TestClass]
public sealed class ProgramExecutionTest
{
    [TestMethod]
    public void Execute_Scalar_Program_Assigns_And_Writes_Values()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Demo; const seed = 2; var count: integer; total: real; enabled: boolean; marker: char; begin count := seed + 3; total := count / 2; enabled := count > 0; marker := 'X'; write(count, ' ', total); writeln(enabled, marker); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("5 2.5TRUEX" + Environment.NewLine, output.Text);
    }

    private sealed class BufferOutput : IOutput
    {
        public string Text { get; private set; } = string.Empty;

        public void Write(string value) => Text += value;

        public void WriteLine(string value) => Text += value + Environment.NewLine;
    }
}