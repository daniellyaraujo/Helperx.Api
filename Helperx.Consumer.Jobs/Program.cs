using Helperx.Consumer.Jobs;
using Helperx.Consumer.Jobs.Services;

IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<Worker>();
                services.AddSingleton(provider =>
                {
                   return new ServiceBusConsumer(context.Configuration);
                });
            })
            .Build();

await host.RunAsync();