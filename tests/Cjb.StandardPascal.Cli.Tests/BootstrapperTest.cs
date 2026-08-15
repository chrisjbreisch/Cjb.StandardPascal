using Cjb.StandardPascal.Application;
using Cjb.StandardPascal.Language.Parser;
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

        Assert.IsInstanceOfType<ConsoleApp>(application);
        Assert.IsInstanceOfType<Scanner>(scanner);
        Assert.IsInstanceOfType<SystemConsole>(console);
        Assert.IsInstanceOfType<Parser>(parser);
        Assert.IsFalse(string.IsNullOrWhiteSpace(bootstrapper.ApplicationName));
    }
}