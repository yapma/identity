using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleRoleBase.Models;
using SimpleRoleBase.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimpleRoleBase.Controllers
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

            return Result.Error();
        }

        [HttpGet("GetAllRoles")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public async Task<Result<List<IdentityRole>>> GetAllRoles()
        {
            var roles = await _roleManager.Roles.Select(x => x).ToListAsync();
            if(roles != null && roles.Count != 0)
                return Result.Success(roles);
            return Result.Error("No role found.");
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
            {
                if (!string.IsNullOrEmpty(model.Role.ToString()))
                    await _userManager.AddToRoleAsync(user, model.Role.ToString());
                return Result.Success();
            }

            return Result.Error(createUserResult.Errors.Select(x => x.Description).ToArray());
        }

        [HttpGet("GetAllUsers")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public async Task<Result<List<User>>> GetAllUsers()
        {
            var users = await _userManager.Users.Select(x => x).ToListAsync();
            if (users != null && users.Count != 0)
                return Result.Success(users);
            return Result.Error("No user found.");
        }
        
        [HttpPost("AssignRoleToUser")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public async Task<Result> AssignRoleToUser(AssignRolesToUserRequestDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
                return Result.NotFound("user not found.");

            List<string> validRolesNames = new List<string>();
            foreach (Guid id in model.RolesId)
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                if(role != null)
                    validRolesNames.Add(role.Name);  
            }

            if (validRolesNames.Count == 0)
                return Result.NotFound("roles not found.");

            var result = await _userManager.AddToRolesAsync(user, validRolesNames);
            if (result.Succeeded)
                return Result.Success();
            
            return Result.Error(result.Errors.Select(x => x.Description).ToArray());
        }

        [HttpPost("DeleteRoleFromUser")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public async Task<Result> DeleteRoleFromUser(DeleteRolesFromUserRequestDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
                return Result.NotFound("user not found.");

            List<string> validRolesNames = new List<string>();
            foreach (Guid id in model.RolesId)
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                if (role != null)
                    validRolesNames.Add(role.Name);
            }

            if (validRolesNames.Count == 0)
                return Result.NotFound("roles not found.");

            var result = await _userManager.RemoveFromRolesAsync(user, validRolesNames);
            if (result.Succeeded)
                return Result.Success();

            return Result.Error(result.Errors.Select(x => x.Description).ToArray());
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

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault())
            };

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
