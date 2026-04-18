using Clew.API;
using Clew.BLL;
using Clew.DAL;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CompanySystem.API
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<LoginDto> _loginValidator;
        

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings,
            IValidator<RegisterDto> registerValidator,
            IValidator<LoginDto> loginValidator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }
        

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            var validationResult = await _registerValidator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var applicationUser = new ApplicationUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.Email.Split('@')[0],
                Email = registerDto.Email,
            };

            IdentityResult result = await _userManager.CreateAsync(applicationUser, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            IdentityResult addRoleResult = await _userManager.AddToRoleAsync(applicationUser, "USER");
            if (!addRoleResult.Succeeded)
            {
                return BadRequest(addRoleResult);
            }
            return Ok("Successfully registered user");
        }
        

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(LoginDto userLoginDto)
        {
            var validationResult = await _loginValidator.ValidateAsync(userLoginDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var user = await _userManager.FindByEmailAsync(userLoginDto.Email);
            if (user is null)
            {
                return Unauthorized("Invalid Email or Password");
            }


            // Check Password
            var result = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);
            if (!result)
            {
                return Unauthorized("Invalid Email or Password");
            }

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName!));
            claims.Add(new Claim(ClaimTypes.Email, user.Email!));

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDto = GenerateToken(claims);
            return Ok(tokenDto);
        }

        private TokenDto GenerateToken(List<Claim> claims)
        {
            var keyFromConfig = _jwtSettings.SecretKey;
            var keyInBytes = Convert.FromBase64String(keyFromConfig);
            var key = new SymmetricSecurityKey(keyInBytes);
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryDateTime = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);

            var jwt = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                signingCredentials: signingCredentials,
                expires: expiryDateTime
                );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            var tokenDto = new TokenDto(token, _jwtSettings.DurationInMinutes);
            return tokenDto;
        }

    }
}
