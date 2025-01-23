using EasyKart.Reviews.Models;
using EasyKart.Reviews.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EasyKart.Reviews.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        IReviewsRepository _reviewsRepository;

        public ReviewsController(IReviewsRepository reviewsRepository)
        {
            _reviewsRepository = reviewsRepository;
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<List<Review>>> Get(Guid productId)
        {
            List<Review> reviews = new List<Review>();
            try
            {
                reviews = await _reviewsRepository.GetReviews(productId);
                return Ok(reviews);
            }
            catch (Exception)
            {
                return StatusCode(500, "Unable to get reviews");                
            }           
        }
    }
}
