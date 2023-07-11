using Azure.Messaging.ServiceBus;
using System.Diagnostics.CodeAnalysis;

namespace Helperx.Consumer.Jobs.Services;

[ExcludeFromCodeCoverage]
public class ServiceBusConsumer
{
    private readonly IConfiguration _configuration;

    public ServiceBusConsumer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task StartAsync()
    {
        await using var client = new ServiceBusClient(_configuration.GetSection("ServiceBusQueue:ConnectionString").Value);
        var receiver = client.CreateReceiver(_configuration.GetSection("ServiceBusQueue:QueueName").Value);

        var messages = await receiver.ReceiveMessagesAsync(maxMessages: 10);
        foreach (var message in messages)
        {
            // Process the received message
            Console.WriteLine($"Received message: {message.Body}");

            //TODO: colocar aqui a chamada ao banco de dados para registrar o job na tabela

            // Complete the message to remove it from the queue
            await receiver.CompleteMessageAsync(message);
        }
    }
}