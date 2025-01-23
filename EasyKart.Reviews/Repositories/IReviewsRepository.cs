using EasyKart.Reviews.Models;

namespace EasyKart.Reviews.Repositories
{
    public interface IReviewsRepository
    {
        Task<List<Review>> GetReviews(Guid productId);
    }
}
