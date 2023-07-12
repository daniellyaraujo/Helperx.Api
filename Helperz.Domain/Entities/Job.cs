using Helperz.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helperz.Domain.Entities
{
    public class Job
    {
        /// <summary>
        /// Id to identify the task.
        /// </summary>
        [Key]
        public int Id { get; }

        /// <summary>
        /// Description of what task have to do.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Informs what kinf of action is the task: Create, Update, Delete and Read them all.
        /// </summary>
        public JobActions Action { get; set; }

        /// <summary>
        /// Indicates whether the user wants to schedule the job.
        /// </summary>
        public bool IsScheduleJob { get; set; }

        /// <summary>
        /// Time that user schedule your task to be executed.
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Status of task
        /// </summary>
        public JobStatus Status { get; set; }
    }
}
