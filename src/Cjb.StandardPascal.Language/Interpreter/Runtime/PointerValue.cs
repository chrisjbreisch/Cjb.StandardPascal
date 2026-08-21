using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Interpreter.Runtime;

public sealed class PointerValue
{
    public object? Value { get; private set; }
    public bool IsDisposed { get; private set; }

    public void Allocate(object value) { Value = value; IsDisposed = false; }
    public void Dispose(SourceSpan span) { RequireLive(span); IsDisposed = true; Value = null; }
    public object Read(SourceSpan span) { RequireLive(span); return Value!; }
    public void Write(object value, SourceSpan span) { RequireLive(span); Value = value; }
    private void RequireLive(SourceSpan span)
    {
        if (Value is null || IsDisposed) { throw new RuntimeException("Pointer is nil or disposed.", span); }
    }
}