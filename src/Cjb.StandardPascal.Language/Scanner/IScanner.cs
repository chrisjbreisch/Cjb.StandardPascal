namespace Cjb.StandardPascal.Language.Scanner;

public interface IScanner
{
    List<Token> ScanTokens(SourceText source);
}