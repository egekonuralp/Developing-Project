using TechStore.Models;

namespace TechStore.Services.Interfaces
{
    public interface IReviewService
    {
        Task<List<Review>> GetByProductIdAsync(int productId);

        Task<bool> HasUserReviewedAsync(int productId, string userId);

        Task AddReviewAsync(int productId, string userId, int rating, string comment);
    }
}
