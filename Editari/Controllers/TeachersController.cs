using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using Editari.Data;
using Editari.Models;

namespace Editari.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Teacher")]
    public class TeachersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeachersController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Diagnostic endpoint: tregon kush je sipas token-it
        // Vetëm Staff duhet ta marrë 200. Parent duhet të marrë 403.
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            return Ok(new
            {
                isAuthenticated = User.Identity?.IsAuthenticated,
                name = User.Identity?.Name,
                role = User.FindFirstValue(ClaimTypes.Role),
                id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetAll()
        {
            return await _context.Teachers.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> Get(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();
            return teacher;
        }

        // ---------------- REGISTER ----------------
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] eDitari.Dtos.CreateTeacherDTO dto)
        {
            if (await _context.Teachers.AnyAsync(t => t.Username == dto.Username))
                return BadRequest("Ky username ekziston tashmë.");

            var teacher = new Teacher
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                Phone = dto.Phone,
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return Ok(new { teacher.TeacherId, teacher.Username });
        }

        // ---------------- ADMIN CREATE ----------------
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Teacher>> Post([FromBody] eDitari.Dtos.CreateTeacherDTO dto)
        {
            var teacher = new Teacher
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                Phone = dto.Phone,
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = teacher.TeacherId }, teacher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Teacher teacher)
        {
            if (id != teacher.TeacherId) return BadRequest();

            _context.Entry(teacher).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            // 1. Remove related RefreshTokens directly
            await _context.RefreshTokens.Where(rt => rt.TeacherId == id).ExecuteDeleteAsync();

            // 2. Remove related Subjects directly
            await _context.Subjects.Where(s => s.TeacherId == id).ExecuteDeleteAsync();

            // 3. Remove related Comments directly
            await _context.Comments.Where(c => c.TeacherId == id).ExecuteDeleteAsync();

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
