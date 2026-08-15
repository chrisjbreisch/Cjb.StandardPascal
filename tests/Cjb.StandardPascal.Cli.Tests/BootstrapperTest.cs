using Cjb.StandardPascal.Application;
using Cjb.StandardPascal.Language.Interpreter;
using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;

using Microsoft.Extensions.DependencyInjection;

namespace Cjb.StandardPascal.Cli.Tests;

[TestClass]
public sealed class BootstrapperTest
{
    [TestMethod]
    public void Bootstrapper_Registers_Console_Application()
    {
        using Bootstrapper bootstrapper = new();

        IConsoleApp application =
            bootstrapper.ScopedServiceProvider.GetRequiredService<IConsoleApp>();
        IScanner scanner =
            bootstrapper.ScopedServiceProvider.GetRequiredService<IScanner>();
        IConsole console =
            bootstrapper.ScopedServiceProvider.GetRequiredService<IConsole>();
        IParser parser =
            bootstrapper.ScopedServiceProvider.GetRequiredService<IParser>();
        IExpressionFormatter formatter =
            bootstrapper.ScopedServiceProvider.GetRequiredService<IExpressionFormatter>();
        IStatementFormatter statementFormatter =
            bootstrapper.ScopedServiceProvider.GetRequiredService<IStatementFormatter>();
        IInterpreter interpreter =
            bootstrapper.ScopedServiceProvider.GetRequiredService<IInterpreter>();

        Assert.IsInstanceOfType<ConsoleApp>(application);
        Assert.IsInstanceOfType<Scanner>(scanner);
        Assert.IsInstanceOfType<SystemConsole>(console);
        Assert.IsInstanceOfType<Parser>(parser);
        Assert.IsInstanceOfType<ExpressionFormatter>(formatter);
        Assert.IsInstanceOfType<StatementFormatter>(statementFormatter);
        Assert.IsInstanceOfType<Interpreter>(interpreter);
        Assert.IsFalse(string.IsNullOrWhiteSpace(bootstrapper.ApplicationName));
    }
}