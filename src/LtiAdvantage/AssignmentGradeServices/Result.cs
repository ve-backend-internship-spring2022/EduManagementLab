﻿using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <summary>
    /// The score result for a single line item and user.
    /// </summary>
    /// <remarks>
    /// There may be multiple scores for a given line item and user,
    /// but only one result. It is up to the platform to decide how
    /// to calculate the result.
    /// </remarks>
    public class Result
    {
        /// <summary>
        /// The endpoint url for this result.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// A comment associated with this result.
        /// </summary>
        [JsonPropertyName("comment")]
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Maximum result score.
        /// </summary>
        [JsonPropertyName("resultMaximum")]
        [JsonProperty("resultMaximum")]
        public double ResultMaximum { get; set; }

        /// <summary>
        /// The line item result.
        /// </summary>
        [JsonPropertyName("resultScore")]
        [JsonProperty("resultScore")]
        public double ResultScore { get; set; }

        /// <summary>
        /// The line item.
        /// </summary>
        [JsonPropertyName("scoreOf")]
        [JsonProperty("scoreOf")]
        public string ScoreOf { get; set; }

        /// <summary>
        /// The user id.
        /// </summary>
        [JsonPropertyName("userId")]
        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}
