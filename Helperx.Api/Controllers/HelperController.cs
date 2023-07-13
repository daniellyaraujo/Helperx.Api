using Helperx.Application.Contracts;
using Helperx.Application.Services;
using Helperz.Application.Contracts;
using Helperz.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Helperx.Api.Controllers
{
    /// <summary>
    /// Controller that provides endpoints for manipulating and creating jobs.
    /// </summary>
    [ApiController]
    [Route("api/v1/job")]
    public class HelperController : ControllerBase
    {
        private readonly IHelperService _helperService;

        /// <summary>
        /// Builder responsible for injecting the necessary classes to be used by the endpoints.
        /// </summary>
        /// <param name="iHelperService"></param>
        public HelperController(IHelperService iHelperService)
        {
            _helperService = iHelperService;
        }

        /// <summary>
        /// Endpoint responsible for querying a list of jobs.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<List<Job>> GetAsync()
        {
            var response = _helperService.GetAllJobsAsync();
            return response;
        }

        /// <summary>
        /// Endpoint responsible for creating a job to run.
        /// </summary>
        /// <param name="jobDescription"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<JobResponse> PostAsync([FromBody] JobRequest jobDescription)
        {
            var response = await _helperService.RegisterJobAsync(jobDescription);
            return response;
        }

        /// <summary>
        /// Endpoint responsible for updating an existing job by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jobDescription"></param>
        /// <returns></returns>
        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<JobResponse> PutAsync([FromRoute] int id, [FromBody] UpdateJobRequest jobDescription)
        {
            var response = await _helperService.UpdateJobByIdAsync(id, jobDescription);
            return response;
        }

        /// <summary>
        /// Endpoint responsible for deleting an existing job by id.
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("remove/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<JobResponse> DeleteAsync([FromRoute] int id)
        {
            var response = await _helperService.RemoveJobByIdAsync(id);
            return response;
        }
    }
}