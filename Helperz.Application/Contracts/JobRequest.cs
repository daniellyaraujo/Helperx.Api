using Helperz.Domain.Enums;

namespace Helperz.Application.Contracts
{
    public class JobRequest
    {
        /// <summary>
        /// Description of what job have to do.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Informs what kinf of action is the job: Create, Update, Delete and Read them all.
        /// </summary>
        public JobActions Action { get; set; }

        /// <summary>
        /// Time that user schedule your job to be executed.
        /// </summary>
        public DateTime ScheduleTime { get; set; }
    }
}