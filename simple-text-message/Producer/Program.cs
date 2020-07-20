using System;
using System.Text;
using RabbitMQ.Client;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting the producer, Please type your message");
            var message = Console.ReadLine();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            var key = "RabbitTest";
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: key,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                    routingKey: key,
                                    basicProperties: null,
                                    body: body);

                Console.WriteLine($"Message '{message}' sent!");
            }

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
            Console.WriteLine("Producer shutting down");
        }
    }
}
