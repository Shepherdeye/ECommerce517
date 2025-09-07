namespace ECommerce517.Repositories
{
    public interface IProductRepository :IRepository<Product>
    {
        Task AddRangeAsync(List<Product> products);
    }
}
