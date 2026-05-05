using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using UniRequestAPI.DbContexts;
using UniRequestAPI.Models.DTO;
using UniRequestAPI.Models.People;
using UniRequestAPI.Models.Users;
using UniRequestAPI.Utils;

namespace UniRequestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ApplicationContext _context;

        public AuthController()
        {
            _context = new ApplicationContext();
        }

        [HttpPost("/register")]
        public IActionResult Register([FromBody] RegisterDto registerDto)
        {
            var existingRequest = _context.RegistrationRequests.FirstOrDefault(r => r.Login == registerDto.Login && r.Status == "pending");
            if (existingRequest != null)
                return BadRequest(new { error = "Registration request with this login already exists and pending approval" });

            var existingUser = _context.Users.FirstOrDefault(u => u.Login == registerDto.Login);
            if (existingUser != null)
                return BadRequest(new { error = "User with this login already exists" });

            var registrationRequest = new RegistrationRequest
            {
                Login = registerDto.Login,
                Password = AuthUtils.HashPassword(registerDto.Password),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                MiddleName = registerDto.MiddleName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Department = registerDto.Department,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };
            _context.RegistrationRequests.Add(registrationRequest);
            _context.SaveChanges();

            return Ok(new
            {
                message = "Registration request submitted successfully. Waiting for admin approval.",
                requestId = registrationRequest.Id
            });
        }

        [HttpPost("/login")]
        public IActionResult Login(string username, string password)
        {
            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password!" });
            }

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name,
            };

            return Ok(JsonConvert.SerializeObject(response));
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Login == username);
            if (user == null || !AuthUtils.VerifyPassword(password, user.Password))
            {
                return null;
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role),
                };

            var claimsIdentity = new ClaimsIdentity(claims, "Token",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}
