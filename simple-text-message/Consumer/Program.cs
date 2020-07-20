using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press [enter] to start the consumer");
            Console.ReadLine();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            var key = "RabbitTest";
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                Console.WriteLine("Listening... press any key to exit.");
                channel.QueueDeclare(queue: key,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine($"Recieving message: {message}");
                };
                channel.BasicConsume(queue: key,
                                    autoAck: true,
                                    consumer: consumer);

                Console.ReadLine();
                Console.WriteLine("Consumer shutting down");
            }
        }
    }
}
