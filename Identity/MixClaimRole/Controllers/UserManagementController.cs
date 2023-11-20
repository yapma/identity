using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MixClaimRole.Models;
using MixClaimRole.Models.DTOs;
using System.Security.Claims;

namespace MixClaimRole.Controllers
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

        [HttpPost("RegisterNewRole")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public async Task<Result> RegisterNewRole(RegisterNewRoleRequestDto model)
        {
            IdentityRole role = new IdentityRole()
            {
                Name = model.Name,
            };

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
                return Result.Success();

            return Result.Error();
        }

        [HttpGet("GetAllRoles")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public async Task<Result<List<IdentityRole>>> GetAllRoles()
        {
            var roles = await _roleManager.Roles.Select(x => x).ToListAsync();
            return Result.Success(roles);
        }

        [HttpGet("GetAllClaims")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public Result<List<Claim>> GetAllClaims()
        {
            var claims = AuthorizationClaims.ToList();
            return Result.Success(claims);
        }

        [HttpPost("AssignClaimsToRole")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public async Task<Result> AssignClaimsToRole(AssignClaimsToRoleRequestDto model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId.ToString());
            if (role == null)
                return Result.NotFound("role not found.");

            var validClaims = new List<string>();
            var claims = AuthorizationClaims.ToList();
            foreach (var item in model.ClaimNames)
            {
                var c = claims.FirstOrDefault(x => x.Type == item);
                if (c != null)
                    await _roleManager.AddClaimAsync(role, c);
            }

            return Result.Success();
        }

        [HttpGet("GetAllUsers")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public async Task<Result<List<User>>> GetAllUsers()
        {
            var users = await _userManager.Users.Select(x => x).ToListAsync();
            return Result.Success(users);
        }

        [HttpPost("RegisterNewUser")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public async Task<Result> RegisterNewUser(RegisterNewUserRequestDto model)
        {
            User user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber,
            };

            var createUserResult = await _userManager.CreateAsync(user, model.Password);
            if (createUserResult.Succeeded)
                return Result.Success();

            return Result.Error();
        }


    }
}
