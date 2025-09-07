using System.Threading.Tasks;

namespace ECommerce517.Repositories
{
    public class ProductRepositry : Repository<Product> ,IProductRepository
    {
        private ApplicationDbContext _context;

        public ProductRepositry(ApplicationDbContext context) : base(context)
        {
            _context=context;
        }

        public async Task AddRangeAsync(List<Product> products)
        {
            await _context.Products.AddRangeAsync(products);
        }
    }
}
