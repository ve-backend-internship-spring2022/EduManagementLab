using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <summary>
    /// Represents a score.
    /// </summary>
    public class Score
    {
        /// <summary>
        /// Status of the user toward activity's completion.
        /// </summary>
        [JsonPropertyName("activityProgress")]
        [JsonProperty("activityProgress")]
        public ActivityProgress ActivityProgress { get; set; }

        /// <summary>
        /// A comment with the score.
        /// </summary>
        [JsonPropertyName("comment")]
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// The status of the grading process.
        /// </summary>
        [JsonPropertyName("gradingProgress")]
        [JsonProperty("gradingProgress")]
        public GradingProgess GradingProgress { get; set; }

        /// <summary>
        /// The score.
        /// </summary>
        [JsonPropertyName("scoreGiven")]
        [JsonProperty("scoreGiven")]
        public double ScoreGiven { get; set; }

        /// <summary>
        /// The maximum possible score.
        /// </summary>
        [JsonPropertyName("scoreMaximum")]
        [JsonProperty("scoreMaximum")]
        public double ScoreMaximum { get; set; }

        /// <summary>
        /// The UTC time the score was set. ISO 8601 format.
        /// </summary>
        [JsonPropertyName("timestamp")]
        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The user id.
        /// </summary>
        [JsonPropertyName("userId")]
        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}
