using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PermissionBase.Common;
using System.ComponentModel;

namespace PermissionBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        [Permission]
        [DisplayName("First Test Api")]
        [HttpGet("TestApi1")]
        [TranslateResultToActionResult]
        public async Task<Result> TestApi1()
        {
            return Result.Success();
        }

        [Permission]
        [DisplayName("Second Test Api")]
        [HttpGet("TestApi2")]
        [TranslateResultToActionResult]
        public async Task<Result> TestApi2()
        {
            return Result.Success();
        }

    }
}
