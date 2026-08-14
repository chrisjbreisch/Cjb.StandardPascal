using Cjb.StandardPascal.Application;

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

        Assert.IsInstanceOfType<ConsoleApp>(application);
        Assert.IsFalse(string.IsNullOrWhiteSpace(bootstrapper.ApplicationName));
    }
}