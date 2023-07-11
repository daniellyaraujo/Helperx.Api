using Helper.Domain.Enums;
using Helperx.Domain.Enums;

namespace Helper.Domain.Entities
{
    public class Job
    {
        /// <summary>
        /// Description of what task have to do.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Informs what kinf of action is the task: Creation, Update, Delete and Read them all.
        /// </summary>
        public JobActions Action { get; set; }

        /// <summary>
        /// Time that user schedule your task to be executed.
        /// </summary>
        public DateTime TimeToExecute { get; set; }

        /// <summary>
        /// Id to identify the task.
        /// </summary>
        public string? Id { get; private set; }

        /// <summary>
        /// Status of task
        /// </summary>
        public JobStatus Status { get; set; }
    }
}
