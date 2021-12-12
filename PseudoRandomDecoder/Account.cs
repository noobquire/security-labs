using System;
using System.Text.Json.Serialization;

namespace PseudoRandomDecoder
{
    public class Account
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("money")]
        public int Money { get; set; }
        [JsonPropertyName("deletionTime")]
        public DateTime DeletionTime { get; set; }
    }
}
