using Messaging.Common;
using RabbitMQ.Client;
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

	while (true)
	{
		//Console.WriteLine("Please write your message and press [Enter]\n\nType nothing and press [Enter] to finish application");
		Console.WriteLine("Please write your name and press [Enter]\n\nType nothing and press [Enter] to finish application");
		var name = Console.ReadLine();

		if (string.IsNullOrWhiteSpace(name))
		{
			break;
		}

		var person = new Person(name);

		var json = JsonSerializer.Serialize(person);

		var messageInBytes = Encoding.UTF8.GetBytes(json);

		await channel.BasicPublishAsync(
			exchange: string.Empty,
			routingKey: queueName,
			body: messageInBytes);

		Console.WriteLine($"Name sent: {name}\n");
	}
}
catch (Exception ex)
{
	Console.WriteLine(ex);
}
