using Helperx.Application.Services;
using Helperz.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Helperx.Api.Controllers
{
    [ApiController]
    [Route("api/v1/job")]
    public class HelperController : BaseController
    {
        private readonly IHelperService _iHelperService;
        
        public HelperController(IHelperService iHelperService)
        {
            _iHelperService = iHelperService;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] JobRequest jobDescription)
        {
            var response = await _iHelperService.SendJobToQueueAsync(jobDescription);
            return Response(response);
        }
        
        [HttpPut("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutAsync ([FromRoute] Guid id, JobRequest jobDescription)
        {
            var response = await _iHelperService.ProcessJobAsync(jobDescription);
            return Response(response);
        }
    }
}