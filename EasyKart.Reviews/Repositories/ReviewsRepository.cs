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

            _cosmosClient = new CosmosClient(_cosmosEndpoint, _cosmosKey);
            _container = _cosmosClient.GetContainer(_databaseId, _containerId);
        }

        public async Task<List<Review>> GetReviews(Guid productId)
        {
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
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return reviews;
        }
    }
}
