using Common_class_Lib;
using MassTransit;

namespace Order_API
{
    public class OrderConsumer : IConsumer<SharedMessage>
    {
        private readonly ILogger<OrderConsumer> _logger;

        public OrderConsumer(ILogger<OrderConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SharedMessage> context)
        {
            var message = context.Message.Value;

            Console.WriteLine("Received message from RabbitMQ: {0}", message);

            // Simulate processing
            _logger.LogInformation("Processing message...");
            await Task.Delay(1000);

            Console.WriteLine("Message processed successfully");
        }
    }
}
