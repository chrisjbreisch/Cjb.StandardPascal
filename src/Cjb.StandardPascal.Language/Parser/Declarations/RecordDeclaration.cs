using Cjb.StandardPascal.Language.Scanner;
namespace Cjb.StandardPascal.Language.Parser.Declarations;
public sealed class RecordDeclaration : Declaration { public RecordDeclaration(Token name, IReadOnlyList<Token> fields, SourceSpan span) : base(span) { Name=name; Fields=fields.ToArray(); } public Token Name { get; } public IReadOnlyList<Token> Fields { get; } }