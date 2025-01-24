

using Newtonsoft.Json;

namespace EasyKart.Reviews.Models
{
    public class Review
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("productId")]
        public Guid ProductId { get; set; }
        public string? ReviewText { get; set; }

        public Guid UserId { get; set; }
    }
}
