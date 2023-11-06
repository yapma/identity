using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SimpleRoleBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("TestApi1")]
        [TranslateResultToActionResult]
        public async Task<Result> TestApi1()
        {
            return Result.Success();
        }

        [Authorize(Roles = "Admin, Owner")]
        [HttpGet("TestApi2")]
        [TranslateResultToActionResult]
        public async Task<Result> TestApi2()
        {
            return Result.Success();
        }
    }
}
