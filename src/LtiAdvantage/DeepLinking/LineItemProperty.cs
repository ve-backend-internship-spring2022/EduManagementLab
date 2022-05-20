using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// Line item definition.
    /// </summary>
    public class LineItemProperty
    {
        /// <summary>
        /// Optional label.
        /// </summary>
        [JsonPropertyName("label")]
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// Optional resource id.
        /// </summary>
        [JsonPropertyName("resourceId")]
        [JsonProperty("resourceId")]
        public string ResourceId { get; set; }

        /// <summary>
        /// Maximum score possible.
        /// </summary>
        [JsonPropertyName("scoreMaximum")]
        [JsonProperty("scoreMaximum")]
        public float? ScoreMaximum { get; set; }

        /// <summary>
        /// Optional tag.
        /// </summary>
        [JsonPropertyName("tag")]
        [JsonProperty("tag")]
        public string Tag { get; set; }
    }
}
