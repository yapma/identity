using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimpleClaimBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        [Authorize(Policy = "Policy1")]
        [HttpGet("TestApi1")]
        [TranslateResultToActionResult]
        public async Task<Result> TestApi1()
        {
            return Result.Success();
        }

        [Authorize(Policy = "Policy2")]
        [HttpGet("TestApi2")]
        [TranslateResultToActionResult]
        public async Task<Result> TestApi2()
        {
            return Result.Success();
        }
    }
}
