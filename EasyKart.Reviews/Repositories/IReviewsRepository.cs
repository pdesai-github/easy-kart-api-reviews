using EasyKart.Reviews.Models;
using Microsoft.Azure.Cosmos;

namespace EasyKart.Reviews.Repositories
{
    public interface IReviewsRepository
    {
        Task<List<Review>> GetReviews(Guid productId);
        Task<ItemResponse<Review>> AddReview(Review review);
    }
}
