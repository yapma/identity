using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MixClaimRole.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        [Authorize(Policy = "WriteEmail")]
        [HttpGet("TestApi1")]
        [TranslateResultToActionResult]
        public async Task<Result> TestApi1()
        {
            return Result.Success();
        }

        [Authorize(Policy = "ReadEmail")]
        [HttpGet("TestApi2")]
        [TranslateResultToActionResult]
        public async Task<Result> TestApi2()
        {
            return Result.Success();
        }
    }
}
