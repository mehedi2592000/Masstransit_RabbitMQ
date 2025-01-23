using Microsoft.EntityFrameworkCore;

namespace Product_API
{
    public class ProductDbContext: DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
            
        }

        public DbSet<Product> products { get; set; }
    }
}
