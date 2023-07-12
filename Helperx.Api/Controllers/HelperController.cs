﻿using Helperx.Application.Services;
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
    public class HelperController : BaseController
    {
        private readonly IHelperService _iHelperService;

        /// <summary>
        /// Builder responsible for injecting the necessary classes to be used by the endpoints.
        /// </summary>
        /// <param name="iHelperService"></param>
        public HelperController(IHelperService iHelperService)
        {
            _iHelperService = iHelperService;
        }

        /// <summary>
        /// Endpoint responsible for creating a job to run.
        /// </summary>
        /// <param name="jobDescription"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] JobRequest jobDescription)
        {
            var response = await _iHelperService.RegisterNewJobAsync(jobDescription);
            return Response(response);
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
            var response = await _iHelperService.GetAllJobsAsync();
            return response;
        }

        /// <summary>
        /// Endpoint responsible for updating an existing job by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jobDescription"></param>
        /// <returns></returns>
        [HttpPut("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutAsync([FromRoute] Guid id, [FromBody] JobRequest jobDescription)
        {
            var response = await _iHelperService.UpdateJobByIdAsync(id, jobDescription);
            return Response(response);
        }

        /// <summary>
        /// Endpoint responsible for deleting an existing job by id.
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("remove/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var response = await _iHelperService.RemoveJobByIdAsync(id);
            return Response(response);
        }
    }
}