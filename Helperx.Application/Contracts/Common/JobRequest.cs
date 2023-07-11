using Helper.Domain.Enums;
using Helperx.Domain.Enums;

namespace Helperx.Application.Contracts.Common
{
    public class JobRequest
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
        public string Id { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public JobStatus Status { get; set; }

        /// <summary>
        /// Sets an ID for the task, to identify it.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetTaskId(string id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            id = Id;
        }
    }
}