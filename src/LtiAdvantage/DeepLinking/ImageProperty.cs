using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// Image.
    /// </summary>
    public class ImageProperty
    {
        /// <summary>
        /// Height in pixels.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Url to the image.
        /// </summary>
        [JsonPropertyName("url")]
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Width in pixels.
        /// </summary>
        public int Width { get; set; }
    }
}
