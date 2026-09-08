using TechStore.Models;

namespace TechStore.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetByProductIdAsync(int productId);

        Task<bool> HasUserReviewedAsync(int productId, string userId);

        Task AddAsync(Review review);

        Task SaveAsync();
    }
}
