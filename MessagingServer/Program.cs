using Messaging.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

const string localhost = "localhost";
const string queueName = "hello";

try
{
	var factory = new ConnectionFactory { HostName = localhost };
	using var connection = await factory.CreateConnectionAsync();
	using var channel = await connection.CreateChannelAsync();

	await channel.QueueDeclareAsync(
		queue: queueName,
		durable: false,
		exclusive: false,
		autoDelete: false,
		arguments: null);

	Console.WriteLine("Waiting for messages...");

	var consumer = new AsyncEventingBasicConsumer(channel);

	consumer.ReceivedAsync += Consumer_MessageReceivedAsync;

	await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

	Console.WriteLine("Press [Enter] to stop listening to messages");
	Console.ReadLine();
}
catch (Exception ex)
{
	Console.WriteLine(ex);
}

static Task Consumer_MessageReceivedAsync(object sender, BasicDeliverEventArgs delivery)
{
	var messageInBytes = delivery.Body.ToArray();

	var jsonMessage = Encoding.UTF8.GetString(messageInBytes);

	var person = JsonSerializer.Deserialize<Person>(jsonMessage);

	Console.WriteLine($"Received name: {person.Name}");

	return Task.CompletedTask;
}