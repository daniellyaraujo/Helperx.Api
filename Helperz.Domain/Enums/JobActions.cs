namespace Helperz.Domain.Enums
{
    public enum JobActions
    {
        /// <summary>
        /// Create the job in queue to be executed.
        /// </summary>
        Create = 1,

        /// <summary>
        /// Updates an existing job in queue.
        /// </summary>
        Update = 2,

        /// <summary>
        /// Delete a job from queue.
        /// </summary>
        Delete = 3,

        /// <summary>
        /// Query the job details.
        /// </summary>
        Read = 4
    }
}
