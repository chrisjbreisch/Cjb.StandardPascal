using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser;

public interface IParser
{
    Expression Parse(List<Token> tokens);

    IStatement ParseStatement(List<Token> tokens);

    Program ParseProgram(List<Token> tokens);
}