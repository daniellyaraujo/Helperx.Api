using Helperz.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Helperz.Domain.Entities
{
    public class Job
    {
        /// <summary>
        /// Id to identify the job.
        /// </summary>
        [Key]
        public int Id { get; }

        /// <summary>
        /// Description of what job have to do.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates whether the user wants to schedule the job.
        /// </summary>
        public bool IsScheduleJob { get; set; }

        /// <summary>
        /// Time that indicates when task be executed.
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Status of job: Pending or Concluded.
        /// </summary>
        public JobStatus Status { get; set; }
    }
}
