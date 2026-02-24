using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Editari.Data;
using Editari.Dtos.Auth;
using Editari.Models; 
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
            var staff = await _context.Staff
                .FirstOrDefaultAsync(s => s.Username == dto.Username);

            if (staff != null)
            {
                var ok = BCrypt.Net.BCrypt.Verify(dto.Password, staff.PasswordHash);
                if (!ok)
                    return Unauthorized("Kredenciale të pasakta.");

                var token = CreateJwtToken(staff.StaffId, staff.Username, staff.Role);

                // ✅ NEW: create + save refresh token for staff
                var staffRefreshToken = Guid.NewGuid().ToString();
                var staffRtEntity = new RefreshToken
                {
                    Token = staffRefreshToken,
                    StaffId = staff.StaffId,
                    ParentId = null,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    IsRevoked = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.RefreshTokens.Add(staffRtEntity);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    accessToken = token,
                    refreshToken = staffRefreshToken, // ✅ NEW
                    role = staff.Role,
                    userType = "Staff"
                });
            }

            var parent = await _context.Set<Parent>()
                .FirstOrDefaultAsync(p => p.Email == dto.Username);

            if (parent != null)
            {
                var ok = BCrypt.Net.BCrypt.Verify(dto.Password, parent.PasswordHash);
                if (!ok)
                    return Unauthorized("Kredenciale të pasakta.");

                var token = CreateJwtToken(parent.ParentId, parent.Email, "Parent");

                // ✅ NEW: create + save refresh token for parent
                var parentRefreshToken = Guid.NewGuid().ToString();
                var parentRtEntity = new RefreshToken
                {
                    Token = parentRefreshToken,
                    ParentId = parent.ParentId,
                    StaffId = null,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    IsRevoked = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.RefreshTokens.Add(parentRtEntity);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    accessToken = token,
                    refreshToken = parentRefreshToken, // ✅ NEW
                    role = "Parent",
                    userType = "Parent"
                });
            }

            return Unauthorized("Kredenciale të pasakta.");
        }
        [HttpPost("refresh")]

public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)

{

    if (string.IsNullOrWhiteSpace(dto.RefreshToken))

        return BadRequest("Refresh token mungon.");
 
    var rt = await _context.RefreshTokens

        .FirstOrDefaultAsync(x => x.Token == dto.RefreshToken);
 
    if (rt == null)

        return Unauthorized("Refresh token i pavlefshëm.");
 
    if (rt.IsRevoked)

        return Unauthorized("Refresh token është revokuar.");
 
    if (rt.ExpiresAt < DateTime.UtcNow)

        return Unauthorized("Refresh token ka skaduar.");
 
    // ✅ Determine user type + create new access token

    if (rt.StaffId.HasValue)

    {

        var staff = await _context.Staff.FirstOrDefaultAsync(s => s.StaffId == rt.StaffId.Value);

        if (staff == null) return Unauthorized("Përdoruesi nuk u gjet.");
 
        var newAccessToken = CreateJwtToken(staff.StaffId, staff.Username, staff.Role);
 
        return Ok(new

        {

            accessToken = newAccessToken,

            role = staff.Role,

            userType = "Staff"

        });

    }
 
    if (rt.ParentId.HasValue)

    {

        var parent = await _context.Set<Editari.Models.Parent>()

            .FirstOrDefaultAsync(p => p.ParentId == rt.ParentId.Value);
 
        if (parent == null) return Unauthorized("Përdoruesi nuk u gjet.");
 
        var newAccessToken = CreateJwtToken(parent.ParentId, parent.Email, "Parent");
 
        return Ok(new

        {

            accessToken = newAccessToken,

            role = "Parent",

            userType = "Parent"

        });

    }
 
    return Unauthorized("Refresh token nuk është i lidhur me përdorues.");

}
 
 [HttpPost("logout")]
public async Task<IActionResult> Logout([FromBody] RefreshRequestDto dto)
{
    var rt = await _context.RefreshTokens
        .FirstOrDefaultAsync(x => x.Token == dto.RefreshToken);

    if (rt == null)
        return NotFound("Refresh token nuk u gjet.");

    rt.IsRevoked = true;
    await _context.SaveChangesAsync();

    return Ok("Logout u krye me sukses.");
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