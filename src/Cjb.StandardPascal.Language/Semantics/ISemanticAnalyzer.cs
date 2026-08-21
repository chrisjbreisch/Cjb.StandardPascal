using Cjb.StandardPascal.Language.Parser;

namespace Cjb.StandardPascal.Language.Semantics;

public interface ISemanticAnalyzer
{
    void Analyze(Program program);
}