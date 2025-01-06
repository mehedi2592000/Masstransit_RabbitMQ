using Common_class_Lib;
using MassTransit;
using MassTransit.Testing;

namespace Order_API.Product_Consumer
{
    public class ListProductCreateConsumer : IConsumer<ProductsBatchEvent>
    {
        public async Task Consume(ConsumeContext<ProductsBatchEvent> context)
        {

            try
            {
                
                var products = context.Message.product;
                

                Console.WriteLine($"Received batch of {products.Count} products.");


                foreach (var product in products)
                {
                    Console.WriteLine($"Processing Product: {product.ProductId}, Name: {product.ProductName}, Price: {product.Price}");
                    // Simulate processing
                    await Task.Delay(500);
                }

                //int[] arr = new int[2];

                //arr[0]=1;
                //arr[1]=2;
                //arr[2]=3;
                //arr[4]=4;
                Console.WriteLine("Batch processed successfully");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
