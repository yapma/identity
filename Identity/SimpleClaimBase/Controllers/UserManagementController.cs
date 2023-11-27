using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleClaimBase.Models;
using SimpleClaimBase.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimpleClaimBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public UserManagementController(
            UserManager<User> userManager,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet("GetAllClaims")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public Result<List<Claim>> GetAllClaims()
        {
            var claims = AuthorizationClaims.ToList();
            return Result.Success(claims);
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

        [HttpPost("AddClaimToUser")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public async Task<Result> AddClaimToUser(AddClaimToUserDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return Result.NotFound("user not found.");

            List<Claim> claims = new List<Claim>();
            foreach (var item in model.ClaimNames)
            {
                var claim = AuthorizationClaims.ToList().FirstOrDefault(x => x.Type == item);
                if (claim != null)
                    claims.Add(claim);
            }

            var result = await _userManager.AddClaimsAsync(user, claims);

            if (result.Succeeded)
            {
                return Result.Success();
            }

            return Result.Error();
        }

        [HttpPost("DeleteClaimFromUser")]
        [TranslateResultToActionResult]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
        public async Task<Result> DeleteClaimFromUser(DeleteClaimFromUserDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return Result.NotFound("user not found.");

            List<Claim> claims = new List<Claim>();
            foreach (var item in model.ClaimNames)
            {
                var claim = AuthorizationClaims.ToList().FirstOrDefault(x => x.Type == item);
                if (claim != null)
                    claims.Add(claim);
            }

            var result = await _userManager.RemoveClaimsAsync(user, claims);

            if (result.Succeeded)
                return Result.Success();

            return Result.Error();
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

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims = claims.Concat(userClaims.ToList()).ToList();

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
