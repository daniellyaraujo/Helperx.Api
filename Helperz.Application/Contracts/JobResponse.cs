namespace Helperz.Application.Contracts
{
    public class JobResponse
    {
        public string Message { get; set; }
        public Domain.Enums.JobStatus Status { get; set; }
    }
}