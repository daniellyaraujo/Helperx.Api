using Helperx.Application.ConsumerServices;
using Helperx.Consumer.Jobs;
using Helperx.Consumer.Jobs.Services.Interfaces;

IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<Worker>();
                services.AddSingleton<IListenerService, ListenerService>();
            })
            .Build();

await host.RunAsync();