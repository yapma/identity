using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleClaimBase.Models;
using System.Security.Claims;

namespace SimpleClaimBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public UserManagementController(
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager
            )
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet("GetAllClaims")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public Result<List<Claim>> GetAllClaims()
        {
            var claims = AuthorizationClaims.ToList();
            return Result.Success(claims);
        }
    }
}
