using Helperz.Application.Contracts;
using Helperz.Domain.Enums;

namespace Helperx.Application.Contracts
{
    public class UpdateJobRequest : JobRequest
    {
        /// <summary>
        /// Indicates job status: 1 Pending, 2 Concluded or 3 Late.
        /// </summary>
        public JobStatus CompletedJob { get; set; }
    }
}