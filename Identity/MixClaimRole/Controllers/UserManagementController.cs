using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MixClaimRole.Extentions;
using MixClaimRole.Models;
using MixClaimRole.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MixClaimRole.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public UserManagementController(
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,
            IConfiguration configuration
            )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
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

            return Result.Error(result.GetErrorsDescription());
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

            return Result.Error(createUserResult.GetErrorsDescription());
        }

        [HttpPost("AssignRoleToUser")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public async Task<Result> AssignRoleToUser(AssignRoleToUserRequestDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
                return Result.NotFound("user not found.");

            var validRoles = new List<IdentityRole>();
            foreach (var roleId in model.RolesId)
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if(role != null)
                    validRoles.Add(role);
            }

            if (validRoles == null || validRoles.Count == 0)
                return Result.NotFound("any valid role not found.");

            var result = await _userManager.AddToRolesAsync(user, validRoles.Select(x => x.Name));
            if (result.Succeeded)
                return Result.Success();
            
            return Result.Error(result.GetErrorsDescription());
        }

        [HttpPost("DeleteRoleFromUser")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public async Task<Result> DeleteRoleFromUser(DeleteRoleFromUserRequestDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
                return Result.NotFound("user not found.");

            var validRoles = new List<IdentityRole>();
            foreach (var roleId in model.RolesId)
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role != null)
                    validRoles.Add(role);
            }

            if (validRoles == null || validRoles.Count == 0)
                return Result.NotFound("any valid role not found.");

            var result = await _userManager.RemoveFromRolesAsync(user, validRoles.Select(x => x.Name));
            if (result.Succeeded)
                return Result.Success();

            return Result.Error(result.GetErrorsDescription());
        }

        [HttpPost("Login")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public async Task<Result<LoginResponseDto>> Login(LoginRequestDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return Result.NotFound("username or password incorect.");
            
            var passwordCheck = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordCheck)
                return Result.NotFound("username or password incorect.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || roles.Count == 0)
                return Result.NotFound("user have not any role.");
            

            var rolesClaims = new List<Claim>();
            foreach (var role in roles)
                rolesClaims.AddRange(await _roleManager.GetClaimsAsync(await _roleManager.FindByNameAsync(role)));

            claims = claims.Concat(rolesClaims.ToList()).ToList();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Result.Success(new LoginResponseDto()
            {
                Token = tokenString
            });
        }
    }
}
