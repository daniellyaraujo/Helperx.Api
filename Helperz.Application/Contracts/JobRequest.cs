using Helperz.Domain.Enums;
using Helperz.Domain.Enums;
using Newtonsoft.Json;

namespace Helperz.Application.Contracts
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
    }
}