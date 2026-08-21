using Cjb.StandardPascal.Language.Scanner;
namespace Cjb.StandardPascal.Language.Parser.Expressions;
public sealed class Nil : Expression { public Nil(SourceSpan span) : base(span) { } public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitNilExpression(this); }