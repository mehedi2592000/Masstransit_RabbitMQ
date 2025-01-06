using Common_class_Lib;
using MassTransit;

namespace Order_API.Product_Consumer
{
    public class ProductCreatedConsumer:IConsumer<ProductCreatedEvent>
    {
        private readonly ILogger<ProductCreatedConsumer> _logger;

        public ProductCreatedConsumer(ILogger<ProductCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
        {
            var product = context.Message;

            try
            {
                Console.WriteLine($"Received Product: {product.ProductId}, Name: {product.ProductName}, Price: {product.Price}");
                //_logger.LogInformation($"Product received: {product.ProductId}");

                // Simulate processing
                await Task.Delay(1000);

                // If processing is successful
                Console.WriteLine("Product processed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}");
                throw; // Let MassTransit handle retries and dead-lettering
            }
        }
    }
}
