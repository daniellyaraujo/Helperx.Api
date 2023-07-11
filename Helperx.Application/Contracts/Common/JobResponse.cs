namespace Helperx.Application.Contracts.Common
{
    public class JobResponse
    {
        public string Message { get; set; }
        public Helperz.Domain.Enums.JobStatus Status { get; set; }
    }
}