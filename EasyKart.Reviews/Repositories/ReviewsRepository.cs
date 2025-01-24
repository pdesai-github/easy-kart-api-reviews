using EasyKart.Reviews.Models;
using Microsoft.Azure.Cosmos;

namespace EasyKart.Reviews.Repositories
{
    public class ReviewsRepository : IReviewsRepository
    {
        IConfiguration _configuration;
        private readonly string _cosmosEndpoint;
        private readonly string _cosmosKey;
        private readonly string _databaseId;
        private readonly string _containerId;
        private readonly string _partitionKey;

        private readonly CosmosClient _cosmosClient;
        private readonly Microsoft.Azure.Cosmos.Container _container;


        public ReviewsRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _cosmosEndpoint = _configuration["CosmosDB:endpoint"];
            _cosmosKey = _configuration["CosmosDB:authKey"];
            _databaseId = _configuration["CosmosDB:databaseId"];
            _containerId = _configuration["CosmosDB:containerId"];
            _partitionKey = _configuration["CosmosDB:partitionKey"];

            Console.WriteLine("CosmosDB:endpoint -> "+ _cosmosEndpoint);
            Console.WriteLine("CosmosDB:authKey -> " + _cosmosKey);
            Console.WriteLine("CosmosDB:databaseId -> " + _databaseId);
            Console.WriteLine("CosmosDB:containerId -> " + _containerId);
            Console.WriteLine("CosmosDB:partitionKey -> " + _partitionKey);

            _cosmosClient = new CosmosClient(_cosmosEndpoint, _cosmosKey);
            _container = _cosmosClient.GetContainer(_databaseId, _containerId);
        }

        public async Task<List<Review>> GetReviews(Guid productId)
        {
            Console.WriteLine("[Info:GetReviews] " + productId);
            List<Review> reviews = new List<Review>();

            try
            {
                var query = $"SELECT * FROM c WHERE c.productId = @productIdValue";
                var queryDefinition = new QueryDefinition(query).WithParameter("@productIdValue", productId);

                var queryResultIterator = _container.GetItemQueryIterator<Review>(queryDefinition);

                if (queryResultIterator.HasMoreResults)
                {
                    var res = await queryResultIterator.ReadNextAsync();
                    reviews = res.Resource.ToList();
                    Console.WriteLine("[Info:GetReviews:Count] " + reviews.Count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR:GetReviews] "+ ex.InnerException);
                throw;
            }
            return reviews;
        }

        public async Task<ItemResponse<Review>> AddReview(Review review)
        {
            if (review == null)
                throw new ArgumentNullException(nameof(review), "Review cannot be null");

            try
            {
                review.Id = Guid.NewGuid();
                review.UserId = Guid.Parse("d8e1c062-4d3e-4326-9f16-31b28f62a4c5");
                return await _container.CreateItemAsync(review, new PartitionKey(review.ProductId.ToString()));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[AddReview] Error: {ex.Message}");
                throw;
            }
        }

    }
}
