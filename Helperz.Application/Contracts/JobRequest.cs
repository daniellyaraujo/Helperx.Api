namespace Helperz.Application.Contracts
{
    /// <summary>
    /// Indicates which properties must be inform to the system to perform the job.
    /// </summary>
    public class JobRequest
    {
        /// <summary>
        /// Description of what job have to do.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates whether the user wants to schedule the job.
        /// </summary>
        public bool IsScheduleJob { get; set; }

        /// <summary>
        /// Time that user schedule your job to be executed.
        /// </summary>
        public DateTime? ExecutionTime { get; set; }
    }
}