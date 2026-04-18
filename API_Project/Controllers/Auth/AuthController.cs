using Clew.API;
using Clew.BLL;
using Clew.Common;
using Clew.DAL;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Clew.API
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IErrorMapper _errorMapper;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IOptions<JwtSettings> jwtSettings,
            IValidator<RegisterDto> registerValidator,
            IValidator<LoginDto> loginValidator,
            IErrorMapper errorMapper)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _errorMapper = errorMapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<GeneralResult<bool>>> Register(RegisterDto registerDto)
        {
            var validationResult = await _registerValidator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
            {
                var errors = _errorMapper.MapError(validationResult);
                return BadRequest(GeneralResult<bool>.FailResult(errors));
            }

            var applicationUser = new ApplicationUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.Email.Split('@')[0],
                Email = registerDto.Email,
            };

            var createResult = await _userManager.CreateAsync(applicationUser, registerDto.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(GeneralResult<bool>.FailResult("Failed to create user."));
            }

            var addRoleResult = await _userManager.AddToRoleAsync(applicationUser, "User");
            if (!addRoleResult.Succeeded)
            {
                return BadRequest(GeneralResult<bool>.FailResult("User created but role assignment failed."));
            }

            return Ok(GeneralResult<bool>.SuccessResult(true, "Successfully registered user"));
        }

        [HttpPost("login")]
        public async Task<ActionResult<GeneralResult<TokenDto>>> Login(LoginDto userLoginDto)
        {
            var validationResult = await _loginValidator.ValidateAsync(userLoginDto);
            if (!validationResult.IsValid)
            {
                var errors = _errorMapper.MapError(validationResult);
                return BadRequest(GeneralResult<TokenDto>.FailResult(errors));
            }

            var user = await _userManager.FindByEmailAsync(userLoginDto.Email);
            if (user is null)
            {
                return Unauthorized(GeneralResult<TokenDto>.FailResult("Invalid email or password."));
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);
            if (!passwordValid)
            {
                return Unauthorized(GeneralResult<TokenDto>.FailResult("Invalid email or password."));
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDto = GenerateToken(claims);
            return Ok(GeneralResult<TokenDto>.SuccessResult(tokenDto, "Login successful"));
        }

        private TokenDto GenerateToken(List<Claim> claims)
        {
            var keyInBytes = Convert.FromBase64String(_jwtSettings.SecretKey);
            var key = new SymmetricSecurityKey(keyInBytes);
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryDateTime = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);

            var jwt = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                signingCredentials: signingCredentials,
                expires: expiryDateTime);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new TokenDto(token, _jwtSettings.DurationInMinutes);
        }
    }
}
