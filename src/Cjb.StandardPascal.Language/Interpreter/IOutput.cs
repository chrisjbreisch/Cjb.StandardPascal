namespace Cjb.StandardPascal.Language.Interpreter;

public interface IOutput
{
    void Write(string value);

    void WriteLine(string value);
}