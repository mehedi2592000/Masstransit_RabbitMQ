using Common_class_Lib;
using MassTransit;

namespace Order_API.Product_Consumer
{
    public class GetOrderConsumer : IConsumer<GetOrderListRequest>
    {
        public async Task Consume(ConsumeContext<GetOrderListRequest> context)
        {
            var request = context.Message;
            Console.WriteLine($"Received request for order list. CustomerName filter: {request.orderId}");

            // Simulate fetching order list (replace with real database logic)
            var orders = new List<OrderDto>
            {
                new OrderDto
                {
                    OrderId = 1,
                    CustomerName = "John Doe",
                    TotalAmount = 100.50m,
                    OrderDate = DateTime.Now.AddDays(-2)
                },
                new OrderDto
                {
                    OrderId = 2,
                    CustomerName = "Jane Doe",
                    TotalAmount = 200.75m,
                    OrderDate = DateTime.Now.AddDays(-1)
                }
            };

            // Filter by CustomerName if provided
           

            // Send the response
            await context.RespondAsync(new GetOrderListResponse { Orders = orders });

            Console.WriteLine("Order list sent back to Product Service.");
        }
    }
}
