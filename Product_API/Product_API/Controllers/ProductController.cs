using Common_class_Lib;
using MassTransit;
using MassTransit.Clients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Product_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        //private readonly ISendEndpointProvider _sendEndpointProvider;

        //public ProductController(ISendEndpointProvider sendEndpointProvider)
        //{

        //    _sendEndpointProvider=sendEndpointProvider;
        //}


        //[HttpPost("send")]
        //public async Task<IActionResult> SendMessage()
        //{
        //    string message = "the is the ";

        //    Console.WriteLine($"Sending message: {message}");

        //    var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:order-queue"));

        //    await sendEndpoint.Send(new SharedMessage { Value = message });

        //    Console.WriteLine("Message successfully sent to RabbitMQ");

        //    return Ok("Message sent to RabbitMQ");
        //}


        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<GetOrderListRequest> _requestClient;
        public ProductController(IPublishEndpoint publishEndpoint, IRequestClient<GetOrderListRequest> requestClient)
        {
            _publishEndpoint = publishEndpoint;
            _requestClient=requestClient;
        }

        [HttpGet("get-order-list")]
        public async Task<IActionResult> GetOrderData()
        {
            var response = await _requestClient.GetResponse<GetOrderListResponse>(new GetOrderListRequest
            {
                orderId = 1
            }, timeout: TimeSpan.FromSeconds(10));

            Console.WriteLine($"Order list received. Total Orders: {response.Message.Orders.Count}");

            // Return the response to the client
            return Ok(response.Message.Orders);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct()
        {           

            Console.WriteLine($"Publishing Product");

            // Publish the event to RabbitMQ
            await _publishEndpoint.Publish(new ProductCreatedEvent()
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Sample Product",
                Price = 100.50m,
                CreatedAt = DateTime.UtcNow
            });

            return Ok($"Product  created and event published.");
        }

        [HttpPost("list-create-batch")]
        public async Task<IActionResult> CreateProductsBatch()
        {
            // Simulating a batch of products
            var products = new List<ProductCreatedEvent>
            {
                new ProductCreatedEvent { ProductId = Guid.NewGuid(), ProductName = "Product A", Price = 100.00m, CreatedAt = DateTime.UtcNow },
                new ProductCreatedEvent { ProductId = Guid.NewGuid(), ProductName = "Product B", Price = 200.50m, CreatedAt = DateTime.UtcNow },
                new ProductCreatedEvent { ProductId = Guid.NewGuid(), ProductName = "Product C", Price = 300.75m, CreatedAt = DateTime.UtcNow }
            };

            // Create a batch event
            var batchEvent = new ProductsBatchEvent { product = products };

            Console.WriteLine($"Publishing batch of {products.Count} products.");
            await _publishEndpoint.Publish(batchEvent); // Publish the batch event

            return Ok($"Batch of {products.Count} products created and published.");
        }
    }
}
