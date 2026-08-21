using Cjb.StandardPascal.Language.Interpreter;
using Cjb.StandardPascal.Language.Interpreter.Runtime;
using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics.Symbols;
using Cjb.StandardPascal.Language.Semantics.Types;

namespace Cjb.StandardPascal.Language.Tests.Interpreter;

[TestClass]
public sealed class ActivationRecordTest
{
    [TestMethod]
    public void Lookup_Uses_Lexical_Parent_And_Local_Shadowing()
    {
        ActivationRecord outer = new("outer");
        outer.Declare(new VariableSymbol("count", PascalTypes.Integer, Span(0)), Integer(1));
        ActivationRecord inner = new("inner", outer);

        RuntimeValue inherited = inner.Lookup("count", Span(5));
        inner.Declare(new VariableSymbol("count", PascalTypes.Real, Span(10)), Real(2.5));
        RuntimeValue local = inner.Lookup("COUNT", Span(15));

        Assert.AreEqual(1L, inherited.Value);
        Assert.AreEqual(PascalTypes.Integer, inherited.Type);
        Assert.AreEqual(2.5, local.Value);
        Assert.AreEqual(PascalTypes.Real, local.Type);
    }

    [TestMethod]
    public void Assign_Incompatible_Value_Throws_Source_Correlated_Runtime_Exception()
    {
        ActivationRecord record = new("main");
        record.Declare(new VariableSymbol("count", PascalTypes.Integer, Span(0)), Integer(1));

        RuntimeException exception = Assert.ThrowsExactly<RuntimeException>(
            () => record.Assign("count", Real(2.5), Span(8)));

        Assert.AreEqual("Cannot assign real to integer 'count'.", exception.Message);
        Assert.AreEqual(Span(8), exception.Span);
    }

    private static RuntimeValue Integer(long value) => new(PascalTypes.Integer, value);

    private static RuntimeValue Real(double value) => new(PascalTypes.Real, value);

    private static SourceSpan Span(int start)
    {
        return new SourceSpan("runtime.pas", start, 5, 1, start + 1);
    }
}