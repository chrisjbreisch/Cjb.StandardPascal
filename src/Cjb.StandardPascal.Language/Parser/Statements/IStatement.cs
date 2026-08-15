namespace Cjb.StandardPascal.Language.Parser.Statements;

public interface IStatement
{
    T Accept<T>(IVisitor<T> visitor);
}