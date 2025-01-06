using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common_class_Lib
{
    public class ProductCreatedEvent
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductsBatchEvent
    { 
        public  List<ProductCreatedEvent> product { get; set; }
    }

    public class GetOrderListRequest
    {
        public int orderId { get; set; } 
    }

    public class GetOrderListResponse
    {
        public List<OrderDto> Orders { get; set; }
    }

    public class OrderDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
    }

}
