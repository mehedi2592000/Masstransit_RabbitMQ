namespace Product_API.Service
{
    public class ProductService
    {
        public ProductDbContext _context;

        public ProductService(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> getListData()
        {
            return _context.products.ToList();

        } 

    }
}
