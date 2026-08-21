using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics;
using Cjb.StandardPascal.Language.Semantics.Symbols;
using Cjb.StandardPascal.Language.Semantics.Types;

namespace Cjb.StandardPascal.Language.Tests.Semantics;

[TestClass]
public sealed class SymbolTableTest
{
    [TestMethod]
    public void Declare_And_Resolve_Identifier_Is_Case_Insensitive()
    {
        SymbolTable symbols = new();
        VariableSymbol variable = new("Total", PascalTypes.Integer, Span(0));

        symbols.Declare(variable);

        Symbol resolved = symbols.Resolve("total", Span(6));

        Assert.AreSame(variable, resolved);
    }

    [TestMethod]
    public void Declare_Duplicate_Identifier_Throws_Source_Correlated_Exception()
    {
        SymbolTable symbols = new();
        symbols.Declare(new VariableSymbol("Total", PascalTypes.Integer, Span(0)));

        SemanticException exception = Assert.ThrowsExactly<SemanticException>(
            () => symbols.Declare(new VariableSymbol("total", PascalTypes.Real, Span(6))));

        Assert.AreEqual("Duplicate declaration of 'total'.", exception.Message);
        Assert.AreEqual(Span(6), exception.Span);
    }

    [TestMethod]
    public void Types_Assignment_Compatibility_Permits_Only_Integer_To_Real_Promotion()
    {
        Assert.IsTrue(PascalTypes.Real.IsAssignmentCompatibleWith(PascalTypes.Integer));
        Assert.IsFalse(PascalTypes.Integer.IsAssignmentCompatibleWith(PascalTypes.Real));
        Assert.IsFalse(PascalTypes.Boolean.IsAssignmentCompatibleWith(PascalTypes.Integer));
    }

    [TestMethod]
    public void RoutineSymbol_Provides_Declared_Signature()
    {
        ParameterSymbol parameter = new("value", PascalTypes.Integer, Span(8), false);
        RoutineSymbol routine = new(
            "Increment",
            [parameter],
            PascalTypes.Integer,
            Span(0));

        Assert.AreEqual("Increment", routine.Name);
        Assert.AreEqual(PascalTypes.Integer, routine.ReturnType);
        Assert.ContainsSingle(routine.Parameters);
        Assert.AreSame(parameter, routine.Parameters[0]);
    }

    private static SourceSpan Span(int start)
    {
        return new SourceSpan("symbols.pas", start, 5, 1, start + 1);
    }
}