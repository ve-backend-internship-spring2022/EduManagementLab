﻿using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <summary>
    /// Represents a gradebook column (e.g. an assignment).
    /// </summary>
    public class LineItem
    {
        /// <summary>
        /// The end date and time.
        /// </summary>
        [JsonPropertyName("endDateTime")]
        [JsonProperty("endDateTime")]
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// The endpoint url for this item.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Optional, human-friendly label for this LineItem suitable for display. 
        /// For example, this label might be used as the heading of a column in a gradebook.
        /// </summary>
        [JsonPropertyName("label")]
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// The resource link id.
        /// </summary>
        [JsonPropertyName("resourceLinkId")]
        [JsonProperty("resourceLinkId")]
        public string ResourceLinkId { get; set; }

        /// <summary>
        /// The non-link resource id.
        /// </summary>
        [JsonPropertyName("resourceId")]
        [JsonProperty("resourceId")]
        public string ResourceId { get; set; }

        /// <summary>
        /// The maximum score allowed.
        /// </summary>
        [JsonPropertyName("scoreMaximum")]
        [JsonProperty("scoreMaximum")]
        public double? ScoreMaximum { get; set; }

        /// <summary>
        /// The start date and time.
        /// </summary>
        [JsonPropertyName("startDateTime")]
        [JsonProperty("startDateTime")]
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// Optional tag.
        /// </summary>
        [JsonPropertyName("tag")]
        [JsonProperty("tag")]
        public string Tag { get; set; }
    }
}
