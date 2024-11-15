using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataConsumerService.Model;
using DataConsumerService.Data;
using Microsoft.Extensions.DependencyInjection;

namespace DataConsumerService.Services
{
    public class DataConsumerService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _scopeFactory;

        public DataConsumerService(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;

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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, args) =>
            {
                // Create a new scope for each message processing
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                    var message = Encoding.UTF8.GetString(args.Body.ToArray());

                    await StoreMessageInDatabase(message, context); // Use the DataContext to store the message data in the DB
                    _channel.BasicAck(args.DeliveryTag, false); 
                }
            };

            _channel.BasicConsume(queue: "FileDataQueue", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //close the RabbitMQ connection and channel when the service stops
            _channel.Close();
            _connection.Close();
            return Task.CompletedTask;
        }

        private async Task StoreMessageInDatabase(string message, DataContext context)
        {
            var fileData = new FileData { Name = message };
            context.FileData.Add(fileData);
            await context.SaveChangesAsync(); // Store the message in the database
        }
    }


}
