using Newtonsoft.Json;

namespace EasyKart.Reviews.Models
{
    public class SummaryResponse
    {
        [JsonProperty("status")] // Use JsonPropertyName if using System.Text.Json
        public string Status { get; set; }

        [JsonProperty("summary")] // Use JsonPropertyName if using System.Text.Json
        public string Summary { get; set; }
    }
}
