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
            // 1) Staff login (Teacher/Admin/Staff) - përdor Username
            var staff = await _context.Staff
                .FirstOrDefaultAsync(s => s.Username == dto.Username);

            if (staff != null)
            {
                var ok = BCrypt.Net.BCrypt.Verify(dto.Password, staff.PasswordHash);
                if (!ok)
                    return Unauthorized("Kredenciale të pasakta.");

                var token = CreateJwtToken(staff.StaffId, staff.Username, staff.Role);

                return Ok(new
                {
                    accessToken = token,
                    role = staff.Role,
                    userType = "Staff"
                });
            }

           var parent = await _context.Set<Editari.Models.Parent>()
    .FirstOrDefaultAsync(p => p.Email == dto.Username);


            if (parent != null)
            {
                var ok = BCrypt.Net.BCrypt.Verify(dto.Password, parent.PasswordHash);
                if (!ok)
                    return Unauthorized("Kredenciale të pasakta.");

                var token = CreateJwtToken(parent.ParentId, parent.Email, "Parent");

                return Ok(new
                {
                    accessToken = token,
                    role = "Parent",
                    userType = "Parent"
                });
            }

            // ❗ shumë e rëndësishme
            return Unauthorized("Kredenciale të pasakta.");
        }

        private string CreateJwtToken(int userId, string username, string role)
        {
            var jwt = _config.GetSection("Jwt");
            var key = jwt["Key"]!;
            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
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
