using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// Properties for embedded iframe.
    /// </summary>
    public class IframeProperty
    {
        /// <summary>
        /// Height in pixels.
        /// </summary>
        [JsonPropertyName("height")]
        [JsonProperty("height")]
        public int? Height { get; set; }

        /// <summary>
        /// URL to use as src of the iframe.
        /// </summary>
        [JsonPropertyName("src")]
        [JsonProperty("src")]
        public string Src { get; set; }

        /// <summary>
        /// Width in pixels.
        /// </summary>
        [JsonPropertyName("width")]
        [JsonProperty("width")]
        public int? Width { get; set; }
    }
}
