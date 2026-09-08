using TechStore.Models;
using TechStore.Repositories.Interfaces;
using TechStore.Services.Interfaces;

namespace TechStore.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<List<Review>> GetByProductIdAsync(int productId)
        {
            return await _reviewRepository.GetByProductIdAsync(productId);
        }

        public async Task<bool> HasUserReviewedAsync(int productId, string userId)
        {
            return await _reviewRepository.HasUserReviewedAsync(productId, userId);
        }

        public async Task AddReviewAsync(int productId, string userId, int rating, string comment)
        {
            if (rating is < 1 or > 5)
            {
                throw new ArgumentException("Puan 1 ile 5 arasında olmalıdır.", nameof(rating));
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new ArgumentException("Yorum boş olamaz.", nameof(comment));
            }

            if (comment.Length > 1000)
            {
                throw new ArgumentException("Yorum 1000 karakterden uzun olamaz.", nameof(comment));
            }

            var alreadyReviewed = await _reviewRepository.HasUserReviewedAsync(productId, userId);

            if (alreadyReviewed)
            {
                throw new InvalidOperationException("Bu ürünü zaten değerlendirdiniz.");
            }

            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Comment = comment.Trim(),
                CreatedDate = DateTime.Now
            };

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveAsync();
        }
    }
}
