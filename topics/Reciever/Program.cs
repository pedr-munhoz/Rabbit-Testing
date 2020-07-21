using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Reciever
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: {0} [binding_key...]",
                                        Environment.GetCommandLineArgs()[0]);
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
                Environment.ExitCode = 1;
                return;
            }

            var factory = new ConnectionFactory() { HostName = "localHost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                Console.WriteLine(" Wainting for messages, press [enter] to exit anytime.");

                channel.ExchangeDeclare(exchange: "topic_logs",
                                        type: "topic");

                var queueName = channel.QueueDeclare().QueueName;

                foreach (var bindingKey in args)
                {
                    channel.QueueBind(queue: queueName,
                                    exchange: "topic_logs",
                                    routingKey: bindingKey);
                }

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;

                    Console.WriteLine($" [x] Recieved ({routingKey}) {message}");
                };
                channel.BasicConsume(queue: queueName,
                                    autoAck: true,
                                    consumer: consumer);

                Console.ReadLine();
            }
        }
    }
}
