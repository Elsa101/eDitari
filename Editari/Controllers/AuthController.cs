using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Editari.Data;
using Editari.Dtos.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Editari.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Username dhe Password janë të detyrueshme.");

            var staff = await _context.Staff
                .FirstOrDefaultAsync(s => s.Username == dto.Username);

            if (staff == null)
                return Unauthorized("Kredenciale të pasakta.");

            // ✅ Për momentin: krahasim direkt.
            // NËSE në DB PasswordHash mban plain password, kjo punon menjëherë.
            if (staff.PasswordHash != dto.Password)
                return Unauthorized("Kredenciale të pasakta.");

            // Nëse Role është bosh, i japim Staff si default
            var role = string.IsNullOrWhiteSpace(staff.Role) ? "Staff" : staff.Role;

            var token = CreateJwtToken(staff.StaffId, staff.Username, role);

            return Ok(new
            {
                accessToken = token,
                role
            });
        }

        private string CreateJwtToken(int staffId, string username, string role)
        {
            var jwt = _config.GetSection("Jwt");
            var key = jwt["Key"]!;
            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, staffId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
