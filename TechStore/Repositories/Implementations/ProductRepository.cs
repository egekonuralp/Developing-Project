using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.DTOs;
using TechStore.Models;
using TechStore.Repositories.Interfaces;

namespace TechStore.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Search/CategoryId/MinPrice/MaxPrice filtrelerini tek yerden uyguluyoruz.
        // GetAllAsync, CountAsync ve GetActiveProductsAsync artık bu metodu kullanıyor.
        private static IQueryable<Product> ApplyFilters(IQueryable<Product> query, ProductFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(p =>
                    p.Name.Contains(filter.Search) ||
                    p.Brand.Contains(filter.Search) ||
                    p.Category.Name.Contains(filter.Search));
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(p =>
                    p.CategoryId == filter.CategoryId.Value);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price <= filter.MaxPrice.Value);
            }

            return query;
        }

        public async Task<List<Product>> GetAllAsync(ProductFilterDto filter)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            query = ApplyFilters(query, filter);

            query = query.OrderByDescending(x => x.Id);

            query = query.Skip((filter.Page - 1) * filter.PageSize);

            query = query.Take(filter.PageSize);

            return await query.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<List<Product>> GetRecentProductsAsync(int count)
        {
            return await _context.Products
                .Include(x => x.Category)
                .OrderByDescending(x => x.Id)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<CategoryProductCountDto>> GetProductCountByCategoryAsync()
        {
            return await _context.Products
                .GroupBy(p => p.Category.Name)
                .Select(g => new CategoryProductCountDto
                {
                    CategoryName = g.Key,
                    ProductCount = g.Count()
                })
                .OrderByDescending(x => x.ProductCount)
                .ToListAsync();
        }

        public async Task<int> CountAsync(ProductFilterDto filter)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            query = ApplyFilters(query, filter);

            return await query.CountAsync();
        }

        public async Task<List<Product>> GetActiveProductsAsync(ProductFilterDto filter)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            query = query.Where(p => p.IsActive);

            query = ApplyFilters(query, filter);

            query = query.OrderByDescending(p => p.Id);

            query = query.Skip((filter.Page - 1) * filter.PageSize);

            query = query.Take(filter.PageSize);

            return await query.ToListAsync();
        }
    }
}