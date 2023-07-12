using System.Net;

namespace Helperz.Application.Contracts
{
    /// <summary>
    /// Indicates which properties are returned by the system after the job runs.
    /// </summary>
    public class JobResponse
    {
        /// <summary>
        /// Returns a short description of what happens with the job.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Informs what is the step of job: Pending, Concluded or Canceled.
        /// </summary>
        public Domain.Enums.JobStatus JobStatus { get; set; }

        /// <summary>
        /// Status code of response.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
    }
}