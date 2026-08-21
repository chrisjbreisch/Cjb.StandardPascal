using Cjb.StandardPascal.Language.Interpreter;
using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics;

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

    [TestMethod]
    public void ParseProgram_Malformed_Block_Throws_Source_Correlated_Parse_Exception()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();

        ParseException exception = Assert.ThrowsExactly<ParseException>(() => parser.ParseProgram(
            scanner.ScanTokens(new SourceText("program Demo; begin writeln(1) end"))));

        Assert.AreEqual("Expected '.' after the program block.", exception.Message);
        Assert.AreEqual(35, exception.Span.Column);
    }

    [TestMethod]
    public void Execute_Program_Division_By_Zero_Throws_Runtime_Exception()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        IInterpreter interpreter = new Language.Interpreter.Interpreter();
        Program program = parser.ParseProgram(scanner.ScanTokens(
            new SourceText("program Demo; begin writeln(1 div 0); end.")));

        RuntimeException exception = Assert.ThrowsExactly<RuntimeException>(
            () => interpreter.Execute(program));

        Assert.AreEqual("Division by zero.", exception.Message);
    }

    [TestMethod]
    public void Execute_Structured_Statements_Controls_Program_Flow()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Flow; var count: integer; begin count := 0; while count < 2 do count := count + 1; repeat count := count + 1 until count = 3; if count = 3 then writeln('ok') else writeln('bad'); for count := 1 to 2 do write(count); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("ok" + Environment.NewLine + "12", output.Text);
    }

    [TestMethod]
    public void Execute_Case_Statement_Selects_Matching_Ordinal_Label()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Choice; var value: integer; begin value := 2; case value of 1: writeln('one'); 2, 3: writeln('many'); else writeln('other'); end; end.")));

        interpreter.Execute(program);

        Assert.AreEqual("many" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Ordinal_Routines_Return_Converted_Values()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Ordinals; begin writeln(ord('A'), chr(66), succ(2), pred(2), round(2.6), trunc(2.6)); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("65B3132" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Goto_Jumps_To_Label_In_The_Same_Block()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Jump; label 10; begin goto 10; writeln('skip'); 10: writeln('done'); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("done" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Enumerated_Type_Uses_Ordinal_Values()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Enum; type Color = (red, green, blue); var color: Color; begin color := green; case color of green: writeln('yes'); else writeln('no'); end; end.")));

        interpreter.Execute(program);

        Assert.AreEqual("yes" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Subrange_Assignment_Outside_Bounds_Throws_Runtime_Exception()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        IInterpreter interpreter = new Language.Interpreter.Interpreter();
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Range; type Small = 1..3; var value: Small; begin value := 4; end.")));

        RuntimeException exception = Assert.ThrowsExactly<RuntimeException>(() => interpreter.Execute(program));

        Assert.AreEqual("Value 4 is outside subrange 1..3.", exception.Message);
    }

    [TestMethod]
    public void Execute_With_Assigns_And_Reads_Record_Field()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Records; type Point = record x: integer; end; var point: Point; begin with point do begin x := 7; writeln(x); end; end.")));

        interpreter.Execute(program);

        Assert.AreEqual("7" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_NonBoolean_If_Condition_Throws_Semantic_Exception()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        IInterpreter interpreter = new Language.Interpreter.Interpreter();
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Invalid; begin if 1 then writeln('no'); end.")));

        SemanticException exception = Assert.ThrowsExactly<SemanticException>(() => interpreter.Execute(program));

        Assert.AreEqual("Condition must be Boolean.", exception.Message);
    }

    private sealed class BufferOutput : IOutput
    {
        public string Text { get; private set; } = string.Empty;

        public void Write(string value) => Text += value;

        public void WriteLine(string value) => Text += value + Environment.NewLine;
    }
}