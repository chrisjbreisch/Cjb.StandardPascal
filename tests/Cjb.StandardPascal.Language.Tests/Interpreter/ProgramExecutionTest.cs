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
    public void Execute_Named_Scalar_Type_Alias_Initializes_Variable()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Aliases; type Currency = real; var amount: Currency; begin amount := 2.5; writeln(amount:2:2); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("2.50" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_WriteLn_Formats_Real_Value_With_Width_And_Precision()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Output; var size: real; begin size := 2.5; writeln('Size: ', size:2:2); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("Size: 2.50" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_WriteLn_Inserts_Spaces_When_Strict_Iso_Spacing_Is_Disabled()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(
            new NullInput(),
            output,
            new InterpreterOptions { StrictIsoSpacing = false });
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Output; begin writeln(1, 2); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("1 2" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Consecutive_Write_Statements_Insert_Space_When_Strict_Iso_Spacing_Is_Disabled()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(
            new NullInput(),
            output,
            new InterpreterOptions { StrictIsoSpacing = false });
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Output; begin write('Value:'); writeln(7); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("Value: 7" + Environment.NewLine, output.Text);
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
    public void Execute_Character_For_Loops_Iterate_Forward_And_Backward()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Alphabet; var character: char; begin for character := 'A' to 'C' do write(character); writeln; for character := 'c' downto 'a' do write(character); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("ABC" + Environment.NewLine + "cba", output.Text);
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

    [TestMethod]
    public void Execute_Assignment_To_For_Control_Variable_Throws_Semantic_Exception()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        IInterpreter interpreter = new Language.Interpreter.Interpreter();
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Invalid; var index: integer; begin for index := 1 to 2 do index := 2; end.")));

        SemanticException exception = Assert.ThrowsExactly<SemanticException>(() => interpreter.Execute(program));

        Assert.AreEqual("Cannot assign to active for control variable 'index'.", exception.Message);
    }

    [TestMethod]
    public void Execute_Goto_Unknown_Label_Throws_Semantic_Exception()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        IInterpreter interpreter = new Language.Interpreter.Interpreter();
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Invalid; label 10; begin goto 20; 10: writeln('no'); end.")));

        SemanticException exception = Assert.ThrowsExactly<SemanticException>(() => interpreter.Execute(program));

        Assert.AreEqual("Goto target '20' is not declared in this block.", exception.Message);
    }

    [TestMethod]
    public void Execute_Downto_Case_And_With_Integrates_Phase2_Features()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Integration; type Level = 1..3; Point = record total: integer; end; var level: Level; index: integer; point: Point; begin with point do total := 0; for index := 3 downto 1 do with point do total := total + index; level := 2; case level of 1: writeln('low'); 2: with point do writeln(total); else writeln('high'); end; end.")));

        interpreter.Execute(program);

        Assert.AreEqual("6" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Procedure_Declaration_And_Call_Writes_Output()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Routines; procedure Hello; begin writeln('hello'); end; begin Hello; end.")));

        interpreter.Execute(program);

        Assert.AreEqual("hello" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Function_With_Value_Parameter_Returns_Result()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Routines; function Twice(value: integer): integer; begin Twice := value * 2; end; begin writeln(Twice(3)); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("6" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Function_Local_Variable_And_Nested_Function_Are_In_Scope()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Functions; function Convert(value: real): real; const Scale = 2.0; begin Convert := value * Scale; end; function Twice(value: real): real; var angle: real; begin angle := Convert(value); Twice := angle; end; begin writeln(Twice(2.5):2:1); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("5.0" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Procedure_With_Var_Parameter_Updates_Caller()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Routines; var count: integer; procedure Increment(var value: integer); begin value := value + 1; end; begin count := 1; Increment(count); writeln(count); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("2" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Recursive_Function_Uses_Isolated_Activations()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Routines; function Factorial(value: integer): integer; begin if value = 0 then Factorial := 1 else Factorial := value * Factorial(value - 1); end; begin writeln(Factorial(5)); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("120" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Forward_Procedure_Uses_Later_Definition()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Routines; procedure Hello; forward; procedure Hello; begin writeln('hello'); end; begin Hello; end.")));

        interpreter.Execute(program);

        Assert.AreEqual("hello" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Nested_Procedure_Resolves_Outer_Local()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Routines; procedure Outer; var value: integer; procedure Inner; begin writeln(value); end; begin value := 4; Inner; end; begin Outer; end.")));

        interpreter.Execute(program);

        Assert.AreEqual("4" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Nested_Procedure_Local_Constant_Shadows_Outer_Constant()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Music; const Scale = 'Bass clef '; var note: char; procedure Tune; const Scale = 'Treble clef '; var note: char; begin note := 'A'; writeln(Scale, note); end; begin note := 'D'; writeln(Scale, note); Tune; writeln(Scale, note); end.")));

        interpreter.Execute(program);

        Assert.AreEqual(
            "Bass clef D" + Environment.NewLine
            + "Treble clef A" + Environment.NewLine
            + "Bass clef D" + Environment.NewLine,
            output.Text);
    }

    [TestMethod]
    public void Execute_Declared_Boolean_Identifier_Is_Valid_If_Condition()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Types; var ready: boolean; begin ready := true; if ready then writeln('yes'); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("yes" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Incompatible_Assignment_Throws_Semantic_Exception()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        IInterpreter interpreter = new Language.Interpreter.Interpreter();
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Types; var count: integer; begin count := true; end.")));

        SemanticException exception = Assert.ThrowsExactly<SemanticException>(() => interpreter.Execute(program));

        Assert.AreEqual("Cannot assign boolean to integer 'count'.", exception.Message);
    }

    [TestMethod]
    public void Execute_Numeric_Predefined_Routines_Return_Expected_Values()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Numeric; begin writeln(abs(-3), sqr(4), sqrt(9), sin(0), cos(0), exp(0), ln(exp(1))); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("31630111" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Array_Index_Assignment_And_Read_Returns_Element()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Arrays; var values: array[1..3] of integer; begin values[2] := 7; writeln(values[2]); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("7" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Array_Index_Outside_Bounds_Throws_Runtime_Exception()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        IInterpreter interpreter = new Language.Interpreter.Interpreter();
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Arrays; var values: array[1..3] of integer; begin values[4] := 7; end.")));

        RuntimeException exception = Assert.ThrowsExactly<RuntimeException>(() => interpreter.Execute(program));

        Assert.AreEqual("Array index 4 is outside 1..3.", exception.Message);
    }

    [TestMethod]
    public void Execute_Multidimensional_Array_Indexing_Returns_Element()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Arrays; var values: array[1..2, 1..2] of integer; begin values[2, 1] := 8; writeln(values[2, 1]); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("8" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Packed_Character_Array_Stores_Characters()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Characters; var letters: packed array[1..2] of char; begin letters[1] := 'A'; letters[2] := 'B'; writeln(letters[1], letters[2]); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("AB" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Packed_Character_Array_Accepts_Exact_Length_String()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Characters; var letters: packed array[1..2] of char; begin letters := 'AB'; writeln(letters[1], letters[2]); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("AB" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Array_Assignment_Uses_Reference_Aliasing()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Arrays; var source, alias: array[1..1] of integer; begin source[1] := 1; alias := source; alias[1] := 2; writeln(source[1]); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("2" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Pack_Copies_Array_Elements()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Packing; var source, target: array[1..2] of integer; begin source[1] := 3; source[2] := 4; pack(source, 1, target); writeln(target[1], target[2]); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("34" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Unpack_Copies_Array_Elements()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Packing; var source, target: array[1..2] of integer; begin source[1] := 5; source[2] := 6; unpack(source, 1, target); writeln(target[1], target[2]); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("56" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Set_Constructor_And_Membership_Returns_Boolean()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Sets; begin writeln(2 in [1, 2, 3]); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("TRUE" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Set_Ranges_And_Operations_Produce_Membership_Result()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Sets; begin writeln(4 in ([1..3] + [4])); writeln(2 in ([1..3] - [2])); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("TRUE" + Environment.NewLine + "FALSE" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_ReadLn_Assigns_Injected_Input()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(new FixedInput("1906"), output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Input; var year: integer; begin readln(year); writeln(year); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("1906" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_ReadLn_Assigns_Multiple_Injected_Inputs()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(new FixedInput("1975 3"), output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Input; var year, position: integer; begin readln(year, position); writeln(year, position); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("19753" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Multiple_Input_Values_Support_Numeric_Expressions()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(
            new FixedInput("8 2"),
            output,
            new InterpreterOptions { StrictIsoSpacing = false });
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Calculations; var cat, rat: integer; begin readln(cat, rat); writeln(cat, rat); writeln(cat + rat, cat - rat); writeln(cat / rat, cat * rat); end.")));

        interpreter.Execute(program);

        Assert.AreEqual(
            "8 2" + Environment.NewLine
            + "10 6" + Environment.NewLine
            + "4 16" + Environment.NewLine,
            output.Text);
    }

    [TestMethod]
    public void Execute_Pointer_New_Dereference_And_Dispose_Uses_Heap_Cell()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Pointers; type IntPointer = ^integer; var pointer: IntPointer; begin new(pointer); pointer^ := 7; writeln(pointer^); dispose(pointer); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("7" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Disposed_Pointer_Dereference_Throws_Runtime_Exception()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        IInterpreter interpreter = new Language.Interpreter.Interpreter();
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Pointers; type IntPointer = ^integer; var pointer: IntPointer; begin new(pointer); dispose(pointer); writeln(pointer^); end.")));

        RuntimeException exception = Assert.ThrowsExactly<RuntimeException>(() => interpreter.Execute(program));

        Assert.AreEqual("Pointer is nil or disposed.", exception.Message);
    }

    [TestMethod]
    public void Execute_Nil_Pointer_Dereference_Throws_Runtime_Exception()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        IInterpreter interpreter = new Language.Interpreter.Interpreter();
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Pointers; type IntPointer = ^integer; var pointer: IntPointer; begin pointer := nil; writeln(pointer^); end.")));

        RuntimeException exception = Assert.ThrowsExactly<RuntimeException>(() => interpreter.Execute(program));

        Assert.AreEqual("Pointer is nil or disposed.", exception.Message);
    }

    [TestMethod]
    public void Execute_Record_Field_Access_Reads_And_Writes_Field()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Records; type Point = record x: integer; end; var point: Point; begin point.x := 9; writeln(point.x); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("9" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Variant_Record_Field_Is_Accessible()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Records; type Choice = record kind: integer; case kind of 1: (value: integer); end; var item: Choice; begin item.value := 5; writeln(item.value); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("5" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_File_Type_Declaration_Initializes_File_Value()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Files; var output: file of integer; begin end.")));

        new Language.Interpreter.Interpreter().Execute(program);
    }

    [TestMethod]
    public void Execute_File_Write_And_Read_Transfers_Item()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Files; var numbers: file of integer; value: integer; begin write(numbers, 12); read(numbers, value); writeln(value); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("12" + Environment.NewLine, output.Text);
    }

    [TestMethod]
    public void Execute_Function_With_Incompatible_Argument_Throws_Runtime_Exception()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        IInterpreter interpreter = new Language.Interpreter.Interpreter();
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Routines; function Twice(value: integer): integer; begin Twice := value * 2; end; begin writeln(Twice('x')); end.")));

        RuntimeException exception = Assert.ThrowsExactly<RuntimeException>(() => interpreter.Execute(program));

        Assert.AreEqual("Argument 1 for 'Twice' must be integer.", exception.Message);
    }

    [TestMethod]
    public void Execute_Local_Variable_Shadowing_Preserves_Outer_Value()
    {
        IScanner scanner = new Language.Scanner.Scanner();
        IParser parser = new Language.Parser.Parser();
        BufferOutput output = new();
        IInterpreter interpreter = new Language.Interpreter.Interpreter(output);
        Program program = parser.ParseProgram(scanner.ScanTokens(new SourceText(
            "program Routines; var value: integer; procedure Show; var value: integer; begin value := 2; writeln(value); end; begin value := 1; Show; writeln(value); end.")));

        interpreter.Execute(program);

        Assert.AreEqual("2" + Environment.NewLine + "1" + Environment.NewLine, output.Text);
    }

    private sealed class BufferOutput : IOutput
    {
        public string Text { get; private set; } = string.Empty;

        public void Write(string value) => Text += value;

        public void WriteLine(string value) => Text += value + Environment.NewLine;
    }

    private sealed class FixedInput : IInput
    {
        private readonly Queue<string> _values;
        public FixedInput(params string[] values) => _values = new Queue<string>(values);
        public string ReadLine() => _values.Dequeue();
    }
}