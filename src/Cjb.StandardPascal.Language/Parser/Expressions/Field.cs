using Cjb.StandardPascal.Language.Scanner;
namespace Cjb.StandardPascal.Language.Parser.Expressions;
public sealed class Field : Expression { public Field(Token record, Token name, SourceSpan span) : base(span) { Record=record; Name=name; } public Token Record { get; } public Token Name { get; } public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitFieldExpression(this); }