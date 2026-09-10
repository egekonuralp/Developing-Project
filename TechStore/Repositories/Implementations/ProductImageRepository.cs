using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;
using TechStore.Repositories.Interfaces;

namespace TechStore.Repositories.Implementations
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductImage>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductImages
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.IsMain)
                .ThenBy(x => x.CreatedDate)
                .ToListAsync();              
        }

        public async Task<ProductImage?> GetByIdAsync(int id)
        {
            return await _context.ProductImages
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(ProductImage image)
        {
            await _context.ProductImages.AddAsync(image);
        }

        public void Delete(ProductImage image)
        {
            _context.ProductImages.Remove(image);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
