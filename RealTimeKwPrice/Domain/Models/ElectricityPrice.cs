using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class ElectricityPrice
    {
        public Guid? Id { get; set; }
        public Guid? UserId { get; set; }

        [JsonPropertyName("SEK_per_kWh")]
        public decimal SEK_per_kWh { get; set; }

        [JsonPropertyName("EUR_per_kWh")]
        public decimal EUR_per_kWh { get; set; }

        [JsonPropertyName("EXR")]
        public decimal EXR { get; set; }

        [JsonPropertyName("time_start")]
        public DateTime time_start { get; set; }

        [JsonPropertyName("time_end")]
        public DateTime time_end { get; set; }
    }
}
