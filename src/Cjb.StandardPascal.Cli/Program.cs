using Cjb.StandardPascal.Application;

using Microsoft.Extensions.DependencyInjection;

using Bootstrapper bootstrapper = new();
IConsoleApp application =
    bootstrapper.ScopedServiceProvider.GetRequiredService<IConsoleApp>();

return application.Run(args);