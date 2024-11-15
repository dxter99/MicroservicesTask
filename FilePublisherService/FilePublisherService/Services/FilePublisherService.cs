using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace FilePublisherService.Services
{
    public class FilePublisherServiceClass : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public FilePublisherServiceClass(IConfiguration configuration)
        {
            _configuration = configuration;

            // Setup RabbitMQ connection and channel
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:HostName"],
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"]
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "FileDataQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void PublishFileData(string filePath)
        {
            // Ensure the file exists before processing
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File at {filePath} not found.");
            }

            // Read the file line by line and send each line to RabbitMQ
            foreach (var line in File.ReadLines(filePath, Encoding.UTF8))
            {
                var body = Encoding.UTF8.GetBytes(line);
                _channel.BasicPublish(exchange: "", routingKey: "FileDataQueue", basicProperties: null, body: body);
            }
        }

        // Dispose of resources (connection and channel) when the service is no longer in use
        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
