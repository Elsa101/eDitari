using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Editari.Data;
using Editari.Models;

namespace Editari.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ParentsController(AppDbContext context)
        {
            _context = context;
        }

        // ---------------- REGISTER ----------------
        public class RegisterParentDto
        {
            public string Name { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterParentDto dto)
        {
            if (await _context.Parents.AnyAsync(p => p.Email == dto.Email))
                return BadRequest("Email already exists.");

            var parent = new Parent
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Parents.Add(parent);
            await _context.SaveChangesAsync();

            return Ok(new { parent.ParentId, parent.Email });
        }

        // ---------------- STAFF CRUD ----------------

        [Authorize(Roles = "Staff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Parent>>> GetAll()
        {
            return await _context.Parents.ToListAsync();
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Parent>> Get(int id)
        {
            var parent = await _context.Parents.FindAsync(id);
            if (parent == null) return NotFound();
            return parent;
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Parent parent)
        {
            if (id != parent.ParentId) return BadRequest();

            _context.Entry(parent).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var parent = await _context.Parents.FindAsync(id);
            if (parent == null) return NotFound();

            _context.Parents.Remove(parent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ---------------- PARENT ENDPOINTS ----------------

        [Authorize(Roles = "Parent")]
        [HttpGet("my-children")]
        public async Task<IActionResult> MyChildren()
        {
            var parentIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(parentIdStr))
                return Unauthorized("ParentId missing in token.");

            var parentId = int.Parse(parentIdStr);

            var children = await _context.StudentParents
                .Where(sp => sp.ParentId == parentId)
                .Select(sp => new
                {
                    sp.Student.StudentId,
                    sp.Student.Name,
                    sp.Student.Surname
                })
                .ToListAsync();

            return Ok(children);
        }

        [Authorize(Roles = "Parent")]
        [HttpGet("my-children/grades")]
        public async Task<IActionResult> MyChildrenGrades()
        {
            var parentIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(parentIdStr))
                return Unauthorized("ParentId missing in token.");

            var parentId = int.Parse(parentIdStr);

            var studentIds = await _context.StudentParents
                .Where(sp => sp.ParentId == parentId)
                .Select(sp => sp.StudentId)
                .ToListAsync();

            var grades = await _context.Grades
                .Where(g => studentIds.Contains(g.StudentId))
                .Select(g => new
                {
                    g.GradeId,
                    g.StudentId,
                    g.Subject,
                    g.GradeValue,
                    g.Date
                })
                .OrderByDescending(g => g.Date)
                .ToListAsync();

            return Ok(grades);
        }
    }
}
