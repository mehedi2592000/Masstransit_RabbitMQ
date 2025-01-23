using Common_class_Lib;
using MassTransit;
using MassTransit.Testing;
using System.Diagnostics;

namespace Order_API.Product_Consumer
{
    public class ListProductCreateConsumer : IConsumer<ProductsBatchEvent>
    {

        public ILogger<ListProductCreateConsumer> _logger;
        public OrderDbContext _context;

        public ListProductCreateConsumer( ILogger<ListProductCreateConsumer> loggerFactory, OrderDbContext context)
        {

            _logger=loggerFactory;
            _context=context;
        }


        public async Task Consume(ConsumeContext<ProductsBatchEvent> context)
        {

            

                var activitySource = new ActivitySource("OrderService");

                // Extract trace headers
                var traceparent = context.Headers.Get<string>("traceparent");
                var tracestate = context.Headers.Get<string>("tracestate");

                var activityContext = traceparent != null
                    ? ActivityContext.Parse(traceparent, tracestate)
                    : default;

                using var activity = activitySource.StartActivity("ConsumeProduct", ActivityKind.Consumer, activityContext);
            try
            {

                var products = context.Message.product;
                

                Console.WriteLine($"Received batch of {products.Count} products.");
                _logger.LogInformation($"Received batch of {products.Count} products.");
                var polist= _context.products.ToList(); 

                foreach (var product in polist)
                {
                    //Console.WriteLine($"Processing Product: {product.ProductId}, Name: {product.ProductName}, Price: {product.Price}");
                    //_logger.LogInformation($"Processing Product: {product.ProductId}, Name: {product.ProductName}, Price: {product.Price}");

                    Console.WriteLine($"Processing Product: {product.Id}, Name: {product.Name}, Price: {product.Description}");
                    _logger.LogInformation($"Processing Product: {product.Id}, Name: {product.Name}, Price: {product.Description}");

                    activity?.SetTag("product.id", product.Id);
                    activity?.SetTag("product.name", product.Name);

                    // Simulate processing
                    await Task.Delay(500);
                }


                //int[] arr = new int[2];

                //arr[0]=1;
                //arr[1]=2;
                //arr[2]=3;
                //arr[4]=4;
                Console.WriteLine("Batch processed successfully");
                _logger.LogInformation("Batch processed successfully");
                activity?.AddEvent(new ActivityEvent("Product processed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError("Batch processed Eror");
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

                // Add the exception details to the activity as an event
                activity?.AddEvent(new ActivityEvent("Exception occurred", default, new ActivityTagsCollection
                {
                    { "error.type", ex.GetType().Name },
                    { "error.message", ex.Message },
                    { "error.stacktrace", ex.StackTrace }
                }));

                throw ;
            }
        }
    }
}
