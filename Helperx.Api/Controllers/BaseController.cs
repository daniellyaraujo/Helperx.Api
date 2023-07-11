using Helperx.Application.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace Helperx.Api.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        //
        // Resumo:
        //     This method returns a ObjectResult, if has any errors then ObjectResult value
        //     is a List of ErrorResponse, otherwise is Object.
        //
        // Devoluções:
        //     A ObjectResult, if it has any errors then the value is List<ErrorResponse>, otherwise
        //     is Object.
        protected new virtual IActionResult Response(JobResponse result) => Response(result);
    }
}