using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics.Symbols;
using Cjb.StandardPascal.Language.Semantics.Types;

namespace Cjb.StandardPascal.Language.Interpreter.Runtime;

public sealed class ActivationRecord
{
    private readonly Dictionary<string, Binding> _bindings =
        new(StringComparer.OrdinalIgnoreCase);

    public ActivationRecord(string name, ActivationRecord? lexicalParent = null)
    {
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("An activation record name is required.", nameof(name))
            : name;
        LexicalParent = lexicalParent;
    }

    public string Name { get; }

    public ActivationRecord? LexicalParent { get; }

    public void Declare(VariableSymbol variable, RuntimeValue value)
    {
        ArgumentNullException.ThrowIfNull(variable);
        ArgumentNullException.ThrowIfNull(value);

        if (_bindings.ContainsKey(variable.Name))
        {
            throw new RuntimeException(
                $"Duplicate runtime binding for '{variable.Name}'.",
                variable.Span);
        }

        _bindings.Add(variable.Name, new Binding(variable, Coerce(variable, value, variable.Span)));
    }

    public RuntimeValue Lookup(string name, SourceSpan span)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return TryFindBinding(name, out Binding? binding) && binding is not null
            ? binding.Value
            : throw new RuntimeException($"Undefined identifier '{name}'.", span);
    }

    public void Assign(string name, RuntimeValue value, SourceSpan span)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(value);

        if (!TryFindBinding(name, out Binding? binding) || binding is null)
        {
            throw new RuntimeException($"Undefined identifier '{name}'.", span);
        }

        binding.Value = Coerce(binding.Variable, value, span);
    }

    private bool TryFindBinding(string name, out Binding? binding)
    {
        if (_bindings.TryGetValue(name, out Binding? localBinding))
        {
            binding = localBinding;
            return true;
        }

        if (LexicalParent is not null)
        {
            return LexicalParent.TryFindBinding(name, out binding);
        }

        binding = null;
        return false;
    }

    private static RuntimeValue Coerce(
        VariableSymbol variable,
        RuntimeValue value,
        SourceSpan span)
    {
        if (!variable.Type.IsAssignmentCompatibleWith(value.Type))
        {
            throw new RuntimeException(
                $"Cannot assign {value.Type.Name} to {variable.Type.Name} '{variable.Name}'.",
                span);
        }

        object? coercedValue = ReferenceEquals(variable.Type, PascalTypes.Real)
            && ReferenceEquals(value.Type, PascalTypes.Integer)
            && value.Value is long integer
                ? (double)integer
                : value.Value;
        return new RuntimeValue(variable.Type, coercedValue);
    }

    private sealed class Binding
    {
        public Binding(VariableSymbol variable, RuntimeValue value)
        {
            Variable = variable;
            Value = value;
        }

        public VariableSymbol Variable { get; }

        public RuntimeValue Value { get; set; }
    }
}