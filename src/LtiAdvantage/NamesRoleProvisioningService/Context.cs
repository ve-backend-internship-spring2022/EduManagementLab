using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LtiAdvantage.NamesRoleProvisioningService
{
    /// <summary>
    /// The context of a membership container.
    /// </summary>
    public class Context
    {
        /// <summary>
        /// The context id.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonProperty("id")]

        public string Id { get; set; }

        /// <summary>
        /// The context label.
        /// </summary>
        [JsonPropertyName("label")]
        [JsonProperty("label")]
        public string Label { get; set; }


        /// <summary>
        /// The context title.
        /// </summary>
        [JsonPropertyName("title")]
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
