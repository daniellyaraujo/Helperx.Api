using Helperz.Domain.Enums;

namespace Helperz.Domain.Entities
{
    public class Job
    {
        /// <summary>
        /// Description of what task have to do.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Informs what kinf of action is the task: Create, Update, Delete and Read them all.
        /// </summary>
        public JobActions Action { get; set; }

        /// <summary>
        /// Time that user schedule your task to be executed.
        /// </summary>
        public DateTime? ScheduleTime { get; set; }

        /// <summary>
        /// Id to identify the task.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Status of task
        /// </summary>
        public JobStatus Status { get; set; }
    }
}
