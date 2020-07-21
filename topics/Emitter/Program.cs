﻿using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace Emitter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "topic_logs",
                                        type: "topic");

                var routingKey = (args.Length > 0) ? args[0] : "anonymous.info";
                var message = (args.Length > 1)
                                ? string.Join(" ", args.Skip(1).ToArray())
                                : "Hello World";
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "topic_logs",
                                    routingKey: routingKey,
                                    basicProperties: null,
                                    body: body);

                Console.WriteLine(" Message sent:");
                Console.WriteLine($" ({routingKey}) {message}");
            }
        }
    }
}
