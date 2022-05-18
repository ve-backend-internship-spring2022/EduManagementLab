using LtiAdvantage.Lti;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// LtiDeepLinkingRequest settings
    /// </summary>
    public class DeepLinkingSettingsClaimValueType
    {
        /// <summary>
        /// The file media types supported.
        /// </summary>
        [JsonPropertyName("accept_media_types")]
        [JsonProperty("accept_media_types")]
        public string AcceptMediaTypes { get; set; }

        /// <summary>
        /// True if the platform accepts multiple items.
        /// </summary>
        [JsonPropertyName("accept_multiple")]
        [JsonProperty("accept_multiple")]
        public bool AcceptMultiple { get; set; }

        /// <summary>
        /// The document targets supported.
        /// </summary>
        [JsonPropertyName("accept_presentation_document_targets")]
        [JsonProperty("accept_presentation_document_targets")]
        public DocumentTarget[] AcceptPresentationDocumentTargets { get; set; }

        /// <summary>
        /// The content types supported.
        /// </summary>
        [JsonPropertyName("accept_types")]
        [JsonProperty("accept_types")]
        public string[] AcceptTypes { get; set; }

        /// <summary>
        /// True if the items are automatically created without confirmation.
        /// </summary>
        [JsonPropertyName("auto_create")]
        [JsonProperty("auto_create")]
        public bool AutoCreate { get; set; }

        /// <summary>
        /// An opaque value which will be returned by the tool.
        /// </summary>
        [JsonPropertyName("data")]
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// URL to send the deep linking response.
        /// </summary>
        [JsonPropertyName("deep_link_return_url")]
        [JsonProperty("deep_link_return_url")]
        public string DeepLinkReturnUrl { get; set; }

        /// <summary>
        /// Default text for items returned.
        /// </summary>
        [JsonPropertyName("text")]
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Default title or alt text for items returned.
        /// </summary>
        [JsonPropertyName("title")]
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
