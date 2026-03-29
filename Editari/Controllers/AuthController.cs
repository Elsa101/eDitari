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
            try
            {
                // Normalize input: Trim whitespace and handle possible nulls
                var normalizedUsername = dto.Username?.Trim() ?? string.Empty;

                // ── 1. Check Staff table (Admin/Staff usually Case-Sensitive but we keep it safe) ──
                var staff = await _context.Staff
                    .FirstOrDefaultAsync(s => s.Username == normalizedUsername);

                if (staff != null && BCrypt.Net.BCrypt.Verify(dto.Password, staff.PasswordHash))
                {
                    var token = CreateJwtToken(staff.StaffId, staff.Username, staff.Role);

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
                        refreshToken = staffRefreshToken,
                        role = staff.Role,
                        userType = "Staff"
                    });
                }

                // ── 2. Check Teacher table (by Username OR Email) - Case-Insensitive ──
                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.Username.ToLower() == normalizedUsername.ToLower() 
                                           || t.Email.ToLower() == normalizedUsername.ToLower());

                if (teacher != null && BCrypt.Net.BCrypt.Verify(dto.Password, teacher.PasswordHash))
                {
                    var token = CreateJwtToken(teacher.TeacherId, teacher.Username, "Teacher");

                    var teacherRefreshToken = Guid.NewGuid().ToString();
                    var teacherRtEntity = new RefreshToken
                    {
                        Token = teacherRefreshToken,
                        TeacherId = teacher.TeacherId,
                        StaffId = null,
                        ParentId = null,
                        ExpiresAt = DateTime.UtcNow.AddDays(7),
                        IsRevoked = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.RefreshTokens.Add(teacherRtEntity);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        accessToken = token,
                        refreshToken = teacherRefreshToken,
                        role = "Teacher",
                        userType = "Teacher"
                    });
                }

                // ── 3. Check Parent table (by Email) - Case-Insensitive ──
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == normalizedUsername.ToLower());

                if (parent != null && BCrypt.Net.BCrypt.Verify(dto.Password, parent.PasswordHash))
                {
                    // Check if parent has any children linked
                    var hasChildren = await _context.StudentParents.AnyAsync(sp => sp.ParentId == parent.ParentId);
                    
                    // IF parent already has children, ensure they are ACTIVE and let them login
                    if (hasChildren)
                    {
                        if (!parent.IsActive) 
                        {
                            parent.IsActive = true; 
                            await _context.SaveChangesAsync();
                        }
                    }
                    else 
                    {
                        // NO children linked: check age
                        var accountAgeDays = (DateTime.UtcNow - parent.CreatedAt).TotalDays;
                        if (accountAgeDays > 14)
                        {
                            parent.IsActive = false;
                            await _context.SaveChangesAsync();
                            return Unauthorized("Llogaria juaj është deaktivizuar sepse nuk keni lidhur asnjë fëmijë brenda 14 ditëve nga regjistrimi.");
                        }
                        
                        // Age is < 14, but still check if they were manually disabled
                        if (!parent.IsActive)
                        {
                             return Unauthorized("Llogaria juaj është deaktivizuar.");
                        }
                    }

                    var token = CreateJwtToken(parent.ParentId, parent.Email, "Parent");
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
                        refreshToken = parentRefreshToken,
                        role = "Parent",
                        userType = "Parent"
                    });
                }

                // ── No match found ──
                return Unauthorized("Kredenciale të pasakta.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message });
            }
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
 
    if (rt.TeacherId.HasValue)
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.TeacherId == rt.TeacherId.Value);
        if (teacher == null) return Unauthorized("Përdoruesi nuk u gjet.");

        var newAccessToken = CreateJwtToken(teacher.TeacherId, teacher.Username, "Teacher");

        return Ok(new
        {
            accessToken = newAccessToken,
            role = "Teacher",
            userType = "Teacher"
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