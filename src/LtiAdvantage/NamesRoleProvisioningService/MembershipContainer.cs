using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LtiAdvantage.NamesRoleProvisioningService
{
    /// <summary>
    /// Represents the results returned by the Membership service.
    /// See https://www.imsglobal.org/spec/lti-nrps/v2p0#membership-container-media-type.
    /// </summary>
    public class MembershipContainer
    {
        /// <summary>
        /// The ID of these results. Typically the request URL to Membership service.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Context information for the list of members.
        /// </summary>
        [JsonPropertyName("context")]
        [JsonProperty("context")]
        public Context Context { get; set; }

        /// <summary>
        /// The list of members in the specified context.
        /// </summary>
        [JsonPropertyName("members")]
        [JsonProperty("members")]
        public ICollection<Member> Members { get; set; }
    }
}
