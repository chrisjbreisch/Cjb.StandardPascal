using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics.Symbols;

namespace Cjb.StandardPascal.Language.Semantics;

public sealed class SymbolTable
{
    private readonly Dictionary<string, Symbol> _symbols =
        new(StringComparer.OrdinalIgnoreCase);

    public void Declare(Symbol symbol)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        if (!_symbols.TryAdd(symbol.Name, symbol))
        {
            throw new SemanticException(
                $"Duplicate declaration of '{symbol.Name}'.",
                symbol.Span);
        }
    }

    public Symbol Resolve(string name, SourceSpan span)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (TryResolve(name, out Symbol? symbol) && symbol is not null)
        {
            return symbol;
        }

        throw new SemanticException($"Undefined identifier '{name}'.", span);
    }

    public bool TryResolve(string name, out Symbol? symbol)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return _symbols.TryGetValue(name, out symbol);
    }
}