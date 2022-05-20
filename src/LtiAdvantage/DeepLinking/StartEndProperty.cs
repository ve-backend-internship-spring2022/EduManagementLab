using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// Start and end date and time.
    /// </summary>
    public class StartEndProperty
    {
        /// <summary>
        /// Optional end date and time.
        /// </summary>
        [JsonPropertyName("endDateTime")]
        [JsonProperty("endDateTime")]
        public DateTime? EndDateTime { get; set; }
        
        /// <summary>
        /// Optional start date and time.
        /// </summary>
        [JsonPropertyName("startDateTime")]
        [JsonProperty("startDateTime")]
        public DateTime? StartDateTime { get; set; }
    }
}
