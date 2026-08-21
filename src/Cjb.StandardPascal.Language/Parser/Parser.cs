using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Parser.Declarations;
using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Parser.Types;
using Cjb.StandardPascal.Language.Parser.Routines;
using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics.Types;

namespace Cjb.StandardPascal.Language.Parser;

public sealed class Parser : IParser
{
    private List<Token> _tokens = [];
    private int _current;

    public Expression Parse(List<Token> tokens)
    {
        Initialize(tokens);

        Expression expression = Expression();
        Consume(TokenType.EndOfFile, "Expected the end of the expression.");
        return expression;
    }

    public IStatement ParseStatement(List<Token> tokens)
    {
        Initialize(tokens);

        IStatement statement = Statement();
        Consume(TokenType.EndOfFile, "Expected the end of the statement.");
        return statement;
    }

    public Program ParseProgram(List<Token> tokens)
    {
        Initialize(tokens);

        if (Match(TokenType.Program))
        {
            Program program = StandardProgram(Previous());
            Consume(TokenType.EndOfFile, "Expected the end of the program.");
            return program;
        }

        IStatement body = Statement();
        Consume(TokenType.EndOfFile, "Expected the end of the program.");
        return new Program(body, body.Span);
    }

    private Program StandardProgram(Token programKeyword)
    {
        Token name = Consume(TokenType.Identifier, "Expected a program name.");
        List<Token> fileParameters = [];

        if (Match(TokenType.LeftParen))
        {
            fileParameters.Add(Consume(TokenType.Identifier, "Expected a file parameter."));

            while (Match(TokenType.Comma))
            {
                fileParameters.Add(Consume(TokenType.Identifier, "Expected a file parameter."));
            }

            Consume(TokenType.RightParen, "Expected ')' after program file parameters.");
        }

        Consume(TokenType.Semicolon, "Expected ';' after program heading.");
        Block block = ParseBlock();
        Token dot = Consume(TokenType.Dot, "Expected '.' after the program block.");
        SourceSpan programSpan = Span(programKeyword, dot);
        return new Program(name, fileParameters, block, programSpan);
    }

    private Block ParseBlock()
    {
        List<Declaration> declarations = [];

        if (Match(TokenType.Label))
        {
            Consume(TokenType.Number, "Expected a numeric label.");
            while (Match(TokenType.Comma)) { Consume(TokenType.Number, "Expected a numeric label."); }
            Consume(TokenType.Semicolon, "Expected ';' after label declarations.");
        }

        if (Match(TokenType.Const))
        {
            while (Check(TokenType.Identifier))
            {
                Token name = Advance();
                Consume(TokenType.Equal, "Expected '=' after constant name.");
                Expression value = Expression();
                Token semicolon = Consume(TokenType.Semicolon, "Expected ';' after constant declaration.");
                declarations.Add(new ConstantDeclaration(name, value, Span(name, semicolon)));
            }
        }

        if (Match(TokenType.Type))
        {
            while (Check(TokenType.Identifier))
            {
                Token name = Advance();
                Consume(TokenType.Equal, "Expected '=' after type name.");
                if (Match(TokenType.Caret))
                {
                    Token caret = Previous();
                    TypeSyntax target = ParseTypeSyntax();
                    Token semicolon = Consume(TokenType.Semicolon, "Expected ';' after type declaration.");
                    declarations.Add(new PointerDeclaration(name, new PointerTypeSyntax(target, Span(caret, target.Span)), Span(name, semicolon)));
                    continue;
                }
                if (Check(TokenType.Number) && _current + 1 < _tokens.Count && _tokens[_current + 1].Type == TokenType.Range)
                {
                    Token minimum = Advance();
                    Advance();
                    Token maximum = Consume(TokenType.Number, "Expected a subrange maximum.");
                    Token semicolon = Consume(TokenType.Semicolon, "Expected ';' after type declaration.");
                    declarations.Add(new SubrangeDeclaration(name, (long)minimum.Literal!, (long)maximum.Literal!, Span(name, semicolon)));
                    continue;
                }

                if (Match(TokenType.Record))
                {
                    List<Token> fields = [];

                    while (!Check(TokenType.End))
                    {
                        fields.Add(Consume(TokenType.Identifier, "Expected a record field name."));
                        while (Match(TokenType.Comma))
                        {
                            fields.Add(Consume(TokenType.Identifier, "Expected a record field name."));
                        }

                        Consume(TokenType.Colon, "Expected ':' after record field names.");
                        ConsumeScalarType();
                        Consume(TokenType.Semicolon, "Expected ';' after record field declaration.");
                    }

                    Token recordEnd = Consume(TokenType.End, "Expected 'end' after record fields.");
                    Token semicolon = Consume(TokenType.Semicolon, "Expected ';' after type declaration.");
                    declarations.Add(new RecordDeclaration(name, fields, Span(name, recordEnd)));
                    continue;
                }

                Consume(TokenType.LeftParen, "Expected '(' to start an enumeration.");
                List<Token> members = [Consume(TokenType.Identifier, "Expected an enumeration member.")];
                while (Match(TokenType.Comma)) { members.Add(Consume(TokenType.Identifier, "Expected an enumeration member.")); }
                Token rightParenthesis = Consume(TokenType.RightParen, "Expected ')' after enumeration members.");
                Consume(TokenType.Semicolon, "Expected ';' after type declaration.");
                declarations.Add(new EnumerationDeclaration(name, members, Span(name, rightParenthesis)));
            }
        }

        if (Match(TokenType.Var))
        {
            while (Check(TokenType.Identifier))
            {
                Token firstName = Advance();
                List<Token> names = [firstName];

                while (Match(TokenType.Comma))
                {
                    names.Add(Consume(TokenType.Identifier, "Expected a variable name."));
                }

                Consume(TokenType.Colon, "Expected ':' after variable names.");
                TypeSyntax type = ParseTypeSyntax();
                Token semicolon = Consume(TokenType.Semicolon, "Expected ';' after variable declaration.");
                declarations.Add(new VariableDeclaration(
                    names,
                    type,
                    Span(firstName, semicolon)));
            }
        }

        while (Match(TokenType.Procedure))
        {
            Token keyword = Previous();
            Token name = Consume(TokenType.Identifier, "Expected a procedure name.");
            IReadOnlyList<RoutineParameter> parameters = ParseRoutineParameters();
            Consume(TokenType.Semicolon, "Expected ';' after procedure heading.");

            if (Match(TokenType.Forward))
            {
                Consume(TokenType.Semicolon, "Expected ';' after forward declaration.");
                continue;
            }

            Block body = ParseBlock();
            Token semicolon = Consume(TokenType.Semicolon, "Expected ';' after procedure declaration.");
            declarations.Add(new ProcedureDeclaration(name, parameters, body, Span(keyword, semicolon)));
        }

        while (Match(TokenType.Function))
        {
            Token keyword = Previous();
            Token name = Consume(TokenType.Identifier, "Expected a function name.");
            IReadOnlyList<RoutineParameter> parameters = ParseRoutineParameters();
            Consume(TokenType.Colon, "Expected ':' before function return type.");
            TypeSyntax returnType = ParseTypeSyntax();
            Consume(TokenType.Semicolon, "Expected ';' after function heading.");
            Block body = ParseBlock();
            Token semicolon = Consume(TokenType.Semicolon, "Expected ';' after function declaration.");
            declarations.Add(new FunctionDeclaration(name, parameters, returnType, body, Span(keyword, semicolon)));
        }

        Token begin = Consume(TokenType.Begin, "Expected 'begin' to start the program block.");
        List<IStatement> statements = [];

        while (!Check(TokenType.End))
        {
            if (Match(TokenType.Semicolon))
            {
                continue;
            }

            statements.Add(Statement());

            if (!Check(TokenType.End))
            {
                Consume(TokenType.Semicolon, "Expected ';' after statement.");
            }
        }

        Token end = Consume(TokenType.End, "Expected 'end' to close the program block.");
        return new Block(declarations, statements, Span(begin, end));
    }

    private IStatement Statement()
    {
        if (Match(TokenType.New, TokenType.Dispose))
        {
            Token keyword = Previous();
            Consume(TokenType.LeftParen, "Expected '(' after allocation routine.");
            Token target = Consume(TokenType.Identifier, "Expected pointer variable.");
            Token rightParenthesis = Consume(TokenType.RightParen, "Expected ')' after pointer variable.");
            return new Allocation(target, keyword.Type == TokenType.Dispose, Span(keyword, rightParenthesis));
        }
        if (Match(TokenType.Read, TokenType.ReadLn))
        {
            Token keyword = Previous();
            Consume(TokenType.LeftParen, "Expected '(' after input routine.");
            Token target = Consume(TokenType.Identifier, "Expected an input target.");
            Token rightParenthesis = Consume(TokenType.RightParen, "Expected ')' after input target.");
            return new Read(target, keyword.Type == TokenType.ReadLn, Span(keyword, rightParenthesis));
        }

        if (Match(TokenType.With))
        {
            Token keyword = Previous();
            Token record = Consume(TokenType.Identifier, "Expected a record variable after 'with'.");
            Consume(TokenType.Do, "Expected 'do' after with record variable.");
            IStatement body = Statement();
            return new With(record, body, Span(keyword, body.Span));
        }

        if (Match(TokenType.Goto))
        {
            Token keyword = Previous();
            Token label = Consume(TokenType.Number, "Expected a numeric goto label.");
            return new Goto(label, Span(keyword, label));
        }

        if (Check(TokenType.Number) && _current + 1 < _tokens.Count && _tokens[_current + 1].Type == TokenType.Colon)
        {
            Token label = Advance();
            Advance();
            IStatement statement = Statement();
            return new Labeled(label, statement, Span(label, statement.Span));
        }
        if (Match(TokenType.Case))
        {
            Token keyword = Previous();
            Expression selector = Expression();
            Consume(TokenType.Of, "Expected 'of' after case selector.");
            List<CaseBranch> branches = [];

            while (!Check(TokenType.Else) && !Check(TokenType.End))
            {
                List<Expression> labels = [Expression()];
                while (Match(TokenType.Comma))
                {
                    labels.Add(Expression());
                }

                Consume(TokenType.Colon, "Expected ':' after case label.");
                IStatement branch = Statement();
                branches.Add(new CaseBranch(labels, branch));
                Match(TokenType.Semicolon);
            }

            IStatement? elseBranch = null;

            if (Match(TokenType.Else))
            {
                elseBranch = Statement();
                Match(TokenType.Semicolon);
            }

            Token end = Consume(TokenType.End, "Expected 'end' after case statement.");
            return new Case(selector, branches, elseBranch, Span(keyword, end));
        }

        if (Match(TokenType.If))
        {
            Token keyword = Previous();
            Expression condition = Expression();
            Consume(TokenType.Then, "Expected 'then' after if condition.");
            IStatement thenBranch = Statement();
            IStatement? elseBranch = Match(TokenType.Else) ? Statement() : null;
            return new If(condition, thenBranch, elseBranch, Span(keyword, (elseBranch ?? thenBranch).Span));
        }

        if (Match(TokenType.While))
        {
            Token keyword = Previous();
            Expression condition = Expression();
            Consume(TokenType.Do, "Expected 'do' after while condition.");
            IStatement body = Statement();
            return new While(condition, body, Span(keyword, body.Span));
        }

        if (Match(TokenType.Repeat))
        {
            Token keyword = Previous();
            List<IStatement> body = [];

            do
            {
                body.Add(Statement());
            }
            while (Match(TokenType.Semicolon) && !Check(TokenType.Until));

            Consume(TokenType.Until, "Expected 'until' after repeat body.");
            Expression condition = Expression();
            return new Repeat(body, condition, Span(keyword, condition.Span));
        }

        if (Match(TokenType.For))
        {
            Token keyword = Previous();
            Token variable = Consume(TokenType.Identifier, "Expected a for control variable.");
            Consume(TokenType.Assign, "Expected ':=' after for control variable.");
            Expression initial = Expression();
            ForDirection direction = Match(TokenType.To)
                ? ForDirection.To
                : Match(TokenType.DownTo)
                    ? ForDirection.DownTo
                    : throw Error(Peek(), "Expected 'to' or 'downto' in for statement.");
            Expression limit = Expression();
            Consume(TokenType.Do, "Expected 'do' after for bounds.");
            IStatement body = Statement();
            return new For(variable, initial, direction, limit, body, Span(keyword, body.Span));
        }

        if (Match(TokenType.Begin))
        {
            _current--;
            return new BlockStatement(ParseBlock());
        }

        if (Match(TokenType.Write, TokenType.WriteLn))
        {
            Token keyword = Previous();
            List<Expression> expressions = [];

            if (Match(TokenType.LeftParen))
            {
                if (!Check(TokenType.RightParen))
                {
                    expressions.Add(Expression());
                    while (Match(TokenType.Comma))
                    {
                        expressions.Add(Expression());
                    }
                }

                Token rightParenthesis = Consume(TokenType.RightParen, "Expected ')' after output arguments.");
                return new Write(expressions, keyword.Type == TokenType.WriteLn, Span(keyword, rightParenthesis));
            }

            return new Write(expressions, keyword.Type == TokenType.WriteLn, keyword.Span);
        }

        if (Check(TokenType.Identifier))
        {
            Token name = Advance();

            if (Match(TokenType.Dot))
            {
                Token field = Consume(TokenType.Identifier, "Expected a record field name.");
                Consume(TokenType.Assign, "Expected ':=' after record field.");
                Expression value = Expression();
                return new FieldAssignment(name, field, value, Span(name, value.Span));
            }

            if (Match(TokenType.Caret))
            {
                if (!Match(TokenType.Assign)) { throw Error(Peek(), "Expected ':=' after pointer dereference."); }
                Expression value = Expression();
                return new DereferenceAssignment(name, value, Span(name, value.Span));
            }

            if (Match(TokenType.LeftBracket))
            {
                List<Expression> subscripts = [Expression()];
                while (Match(TokenType.Comma)) { subscripts.Add(Expression()); }
                Token rightBracket = Consume(TokenType.RightBracket, "Expected ']' after array subscript.");

                if (Match(TokenType.Assign))
                {
                    Expression value = Expression();
                    return new IndexedAssignment(name, subscripts, value, Span(name, value.Span));
                }

                throw Error(rightBracket, "Expected ':=' after indexed variable.");
            }

            if (Match(TokenType.Assign))
            {
                Expression value = Expression();
                return new Assignment(name, value, Span(name, value.Span));
            }

            List<Expression> arguments = [];

            if (Match(TokenType.LeftParen))
            {
                if (!Check(TokenType.RightParen))
                {
                    arguments.Add(Expression());
                    while (Match(TokenType.Comma)) { arguments.Add(Expression()); }
                }

                Token rightParenthesis = Consume(TokenType.RightParen, "Expected ')' after procedure arguments.");
                return new ProcedureCall(name, arguments, Span(name, rightParenthesis));
            }

            return new ProcedureCall(name, arguments, name.Span);
        }

        Token print = Consume(TokenType.Print, "Expected 'Print'.");
        Expression expression = Expression();
        Token semicolon = Consume(
            TokenType.Semicolon,
            "Expected ';' after the Print expression.");

        SourceSpan span = new(
            print.Span.FilePath,
            print.Span.Start,
            semicolon.Span.Start + semicolon.Span.Length - print.Span.Start,
            print.Span.Line,
            print.Span.Column);
        return new Print(expression, span);
    }

    private void Initialize(List<Token> tokens)
    {
        ArgumentNullException.ThrowIfNull(tokens);

        if (tokens.Count == 0 || tokens[^1].Type != TokenType.EndOfFile)
        {
            throw new ArgumentException(
                "The token list must end with an EndOfFile token.",
                nameof(tokens));
        }

        _tokens = tokens;
        _current = 0;
    }

    private Expression Expression()
    {
        Expression left = SimpleExpression();

        if (!Match(
                TokenType.Equal,
                TokenType.NotEqual,
                TokenType.LessThan,
                TokenType.LessThanOrEqual,
                TokenType.GreaterThan,
                TokenType.GreaterThanOrEqual,
                TokenType.In))
        {
            return left;
        }

        Token binaryOperator = Previous();
        Expression right = SimpleExpression();
        return new Binary(left, binaryOperator, right);
    }

    private Expression SimpleExpression()
    {
        Token? sign = Match(TokenType.Plus, TokenType.Minus) ? Previous() : null;
        Expression expression = Term();

        if (sign is not null)
        {
            expression = new Unary(sign, expression);
        }

        while (Match(TokenType.Plus, TokenType.Minus, TokenType.Or))
        {
            Token binaryOperator = Previous();
            Expression right = Term();
            expression = new Binary(expression, binaryOperator, right);
        }

        return expression;
    }

    private Expression Term()
    {
        Expression expression = Factor();

        while (Match(
                   TokenType.Star,
                   TokenType.Slash,
                   TokenType.Div,
                   TokenType.Mod,
                   TokenType.And))
        {
            Token binaryOperator = Previous();
            Expression right = Factor();
            expression = new Binary(expression, binaryOperator, right);
        }

        return expression;
    }

    private Expression Factor()
    {
        if (Match(TokenType.Not))
        {
            Token unaryOperator = Previous();
            return new Unary(unaryOperator, Factor());
        }

        return Primary();
    }

    private Expression Primary()
    {
        if (Match(TokenType.LeftBracket))
        {
            Token leftBracket = Previous();
            List<Expression> elements = [];

            if (!Check(TokenType.RightBracket))
            {
                Expression element = Expression();
                elements.Add(Match(TokenType.Range)
                    ? new SetRange(element, Expression(), Span(leftBracket, Previous()))
                    : element);
                while (Match(TokenType.Comma))
                {
                    element = Expression();
                    elements.Add(Match(TokenType.Range)
                        ? new SetRange(element, Expression(), Span(leftBracket, Previous()))
                        : element);
                }
            }

            Token rightBracket = Consume(TokenType.RightBracket, "Expected ']' after set elements.");
            return new SetLiteral(elements, Span(leftBracket, rightBracket));
        }

        if (Match(TokenType.Number, TokenType.String))
        {
            return new Literal(Previous());
        }

        if (Match(TokenType.Identifier))
        {
            Token name = Previous();

            if (Match(TokenType.Dot))
            {
                Token field = Consume(TokenType.Identifier, "Expected a record field name.");
                return new Field(name, field, Span(name, field));
            }

            if (Match(TokenType.Caret))
            {
                return new Dereference(name, Span(name, Previous()));
            }

            if (Match(TokenType.LeftBracket))
            {
                List<Expression> subscripts = [Expression()];
                while (Match(TokenType.Comma)) { subscripts.Add(Expression()); }
                Token rightBracket = Consume(TokenType.RightBracket, "Expected ']' after array subscript.");
                return new Cjb.StandardPascal.Language.Parser.Expressions.Index(name, subscripts, Span(name, rightBracket));
            }

            if (!Match(TokenType.LeftParen))
            {
                return new Identifier(name);
            }

            List<Expression> arguments = [];

            if (!Check(TokenType.RightParen))
            {
                arguments.Add(Expression());
                while (Match(TokenType.Comma))
                {
                    arguments.Add(Expression());
                }
            }

            Token rightParenthesis = Consume(TokenType.RightParen, "Expected ')' after routine arguments.");
            return new Call(name, arguments, Span(name, rightParenthesis));
        }

        if (Match(TokenType.LeftParen))
        {
            Token leftParenthesis = Previous();
            Expression expression = Expression();
            Token rightParenthesis = Consume(
                TokenType.RightParen,
                "Expected ')' after expression.");
            SourceSpan span = new(
                leftParenthesis.Span.FilePath,
                leftParenthesis.Span.Start,
                rightParenthesis.Span.Start
                    + rightParenthesis.Span.Length
                    - leftParenthesis.Span.Start,
                leftParenthesis.Span.Line,
                leftParenthesis.Span.Column);
            return new Grouping(expression, span);
        }

        throw Error(Peek(), "Expected an expression.");
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type))
        {
            return Advance();
        }

        throw Error(Peek(), message);
    }

    private bool Match(params TokenType[] types)
    {
        if (!types.Any(Check))
        {
            return false;
        }

        Advance();
        return true;
    }

    private bool Check(TokenType type)
    {
        return Peek().Type == type;
    }

    private Token Advance()
    {
        Token current = Peek();

        if (current.Type != TokenType.EndOfFile)
        {
            _current++;
        }

        return current;
    }

    private Token Peek()
    {
        return _tokens[_current];
    }

    private Token Previous()
    {
        return _tokens[_current - 1];
    }

    private static ParseException Error(Token token, string message)
    {
        return new ParseException(message, token.Span);
    }

    private static SourceSpan Span(Token start, Token end)
    {
        return new SourceSpan(
            start.Span.FilePath,
            start.Span.Start,
            end.Span.Start + end.Span.Length - start.Span.Start,
            start.Span.Line,
            start.Span.Column);
    }

    private static SourceSpan Span(Token start, SourceSpan end)
    {
        return new SourceSpan(start.Span.FilePath, start.Span.Start, end.Start + end.Length - start.Span.Start, start.Span.Line, start.Span.Column);
    }

    private Token ConsumeScalarType()
    {
        if (Match(TokenType.Integer, TokenType.Real, TokenType.Boolean, TokenType.Char))
        {
            return Previous();
        }

        throw Error(Peek(), "Expected a scalar type.");
    }

    private TypeSyntax ParseTypeSyntax()
    {
        Match(TokenType.Packed);

        if (Match(TokenType.File))
        {
            Token file = Previous();
            Consume(TokenType.Of, "Expected 'of' after 'file'.");
            TypeSyntax elementType = ParseTypeSyntax();
            return new FileTypeSyntax(elementType, Span(file, elementType.Span));
        }

        if (Match(TokenType.Array))
        {
            Token array = Previous();
            Consume(TokenType.LeftBracket, "Expected '[' after 'array'.");
            List<ArrayBound> bounds = [];
            do
            {
                Token lowerBound = Consume(TokenType.Number, "Expected an array lower bound.");
                Consume(TokenType.Range, "Expected '..' in array bounds.");
                Token upperBound = Consume(TokenType.Number, "Expected an array upper bound.");
                bounds.Add(new ArrayBound((long)lowerBound.Literal!, (long)upperBound.Literal!));
            }
            while (Match(TokenType.Comma));
            Consume(TokenType.RightBracket, "Expected ']' after array bounds.");
            Consume(TokenType.Of, "Expected 'of' after array bounds.");
            TypeSyntax elementType = ParseTypeSyntax();
            return new ArrayTypeSyntax(bounds, elementType, Span(array, elementType.Span));
        }

        if (Match(TokenType.Integer, TokenType.Real, TokenType.Boolean, TokenType.Char))
        {
            Token token = Previous();
            return new ScalarTypeSyntax(token, TypeFor(token));
        }

        return new NamedTypeSyntax(Consume(TokenType.Identifier, "Expected a type name."));
    }

    private IReadOnlyList<RoutineParameter> ParseRoutineParameters()
    {
        List<RoutineParameter> parameters = [];

        if (!Match(TokenType.LeftParen))
        {
            return parameters;
        }

        do
        {
            bool isVariable = Match(TokenType.Var);
            List<Token> names = [Consume(TokenType.Identifier, "Expected a parameter name.")];
            while (Match(TokenType.Comma)) { names.Add(Consume(TokenType.Identifier, "Expected a parameter name.")); }
            Consume(TokenType.Colon, "Expected ':' after parameter names.");
            TypeSyntax type = ParseTypeSyntax();
            parameters.AddRange(names.Select(name => new RoutineParameter(name, type, isVariable)));
        }
        while (Match(TokenType.Semicolon));

        Consume(TokenType.RightParen, "Expected ')' after parameters.");
        return parameters;
    }

    private static PascalType TypeFor(Token token)
    {
        return token.Type switch
        {
            TokenType.Integer => PascalTypes.Integer,
            TokenType.Real => PascalTypes.Real,
            TokenType.Boolean => PascalTypes.Boolean,
            TokenType.Char => PascalTypes.Character,
            _ => throw new ArgumentOutOfRangeException(nameof(token)),
        };
    }
}