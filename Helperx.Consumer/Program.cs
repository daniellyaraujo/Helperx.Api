using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace Helperx.Consumer;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
             .ConfigureLogging((hostBuilderContext, configureLogging) =>
             {
                 configureLogging.ClearProviders();
                 configureLogging.AddLoggerConfiguration(hostBuilderContext.Configuration);
             })
             .ConfigureServices((context, services) =>
             {
                 services.AddHostedService<Worker>();
             })
             .Build();

        await host.RunAsync();
    }
}