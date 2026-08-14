namespace Cjb.StandardPascal.Application;

public sealed class SystemConsole : IConsole
{
    public string? ReadLine()
    {
        return Console.ReadLine();
    }

    public void Write(string value)
    {
        Console.Write(value);
    }

    public void WriteLine(string value)
    {
        Console.WriteLine(value);
    }
}