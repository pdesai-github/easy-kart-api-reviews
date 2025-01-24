using EasyKart.Reviews.Models;
using EasyKart.Reviews.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace EasyKart.Reviews.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        IReviewsRepository _reviewsRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public ReviewsController(IReviewsRepository reviewsRepository, IHttpClientFactory httpClientFactory)
        {
            _reviewsRepository = reviewsRepository;
            _httpClientFactory = httpClientFactory;
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

        [HttpGet("getsummary/{productId}")]
        public async Task<ActionResult<SummaryResponse>> GetSummary(Guid productId)
        {
            List<Review> reviews = new List<Review>();
            string summary = string.Empty;

            try
            {
                reviews = await _reviewsRepository.GetReviews(productId);

                List<string> reviewText = new List<string>();
                foreach (Review review in reviews)
                {
                    reviewText.Add(review.ReviewText);
                }

                if (reviews == null || reviews.Count == 0)
                {
                    return BadRequest("The reviews list cannot be empty.");
                }

                var payload = new { reviews = reviewText };



                var client = _httpClientFactory.CreateClient();
                var url = "http://easy-kart.centralindia.cloudapp.azure.com/summary";

                var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    SummaryResponse summaryResponse = null; // Initialize to null
                    summaryResponse = JsonConvert.DeserializeObject<SummaryResponse>(responseContent);


                    return Ok(summaryResponse);
                }

                return Ok(summary);
            }
            catch (Exception)
            {
                return StatusCode(500, "Unable to get reviews");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ItemResponse<Review>>> Post(Review review)
        {
            var res = await _reviewsRepository.AddReview(review);
            return res;
        }
    }
}
