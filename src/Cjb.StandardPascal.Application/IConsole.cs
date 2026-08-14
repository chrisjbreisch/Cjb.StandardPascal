namespace Cjb.StandardPascal.Application;

public interface IConsole
{
    string? ReadLine();

    void Write(string value);

    void WriteLine(string value);
}