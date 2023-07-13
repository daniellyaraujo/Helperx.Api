using System.Text.Json.Serialization;

namespace Helperz.Domain.Enums
{
    /// <summary>
    /// Informs the job steps during the process.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum JobStatus
    {
        /// <summary>
        /// When job don't be executed yet.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Job become concluded when the job is executed with success.
        /// </summary>
        Concluded = 2
    }
}