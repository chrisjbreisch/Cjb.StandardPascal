using System.Reflection;

using Cjb.StandardPascal.Language.Interpreter;
using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cjb.StandardPascal.Application;

public sealed class Bootstrapper : IDisposable
{
    private readonly ServiceCollection _services = [];
    private readonly IServiceScope _applicationScope;
    private bool _isDisposed;

    public Bootstrapper()
    {
        Configuration = LoadConfiguration();
        ConfigureLogging();
        ConfigureServices();

        RootServiceProvider = _services.BuildServiceProvider(
            new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true,
            });
        _applicationScope = RootServiceProvider.CreateScope();
        ScopedServiceProvider = _applicationScope.ServiceProvider;
        LogFactory = ScopedServiceProvider.GetRequiredService<ILoggerFactory>();
        ApplicationName = GetApplicationName();

        LogConfiguredServices();
    }

    public string ApplicationName { get; }

    public IConfiguration Configuration { get; }

    public ILoggerFactory LogFactory { get; }

    public ServiceProvider RootServiceProvider { get; }

    public IServiceProvider ScopedServiceProvider { get; }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _applicationScope.Dispose();
        RootServiceProvider.Dispose();
        _services.Clear();
        _isDisposed = true;
    }

    private static IConfiguration LoadConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private void ConfigureLogging()
    {
        _services.AddLogging(
            builder => builder
                .ClearProviders()
                .AddConfiguration(Configuration.GetSection("Logging"))
                .AddConsole()
                .AddDebug());
    }

    private void ConfigureServices()
    {
        _services.AddSingleton(Configuration);
        _services.AddSingleton<IConsole, SystemConsole>();
        _services.AddSingleton<IConsoleApp, ConsoleApp>();
        _services.AddSingleton<IScanner, Scanner>();
        _services.AddSingleton<IParser, Parser>();
        _services.AddSingleton<IExpressionFormatter, ExpressionFormatter>();
        _services.AddSingleton<IStatementFormatter, StatementFormatter>();
        _services.AddSingleton<IInterpreter, Interpreter>();
    }

    private void LogConfiguredServices()
    {
        ILogger<Bootstrapper> logger = LogFactory.CreateLogger<Bootstrapper>();

        foreach (ServiceDescriptor service in _services)
        {
            logger.LogTrace(
                "Configured service {ServiceType}: {Implementation}",
                service.ServiceType,
                service.ImplementationType
                    ?? service.ImplementationInstance?.GetType()
                    ?? typeof(object));
        }
    }

    private static string GetApplicationName()
    {
        Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        return assembly.GetName().Name ?? nameof(Cjb.StandardPascal);
    }
}