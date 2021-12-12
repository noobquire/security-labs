using System.Text.Json.Serialization;

namespace PseudoRandomDecoder
{
    public class CasinoResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("account")]
        public Account Account { get; set; }
        [JsonPropertyName("realNumber")]
        public long RealNumber { get; set; }
    }
}
