using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Editari.Data;
using Editari.Models;
using BCrypt.Net;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Parent>>> GetAll()
        {
            return await _context.Parents.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Parent>> Get(int id)
        {
            var parent = await _context.Parents.FindAsync(id);
            if (parent == null) return NotFound();
            return parent;
        }

        [HttpPost]
        public async Task<ActionResult<Parent>> Post(Parent parent)
        {
            _context.Parents.Add(parent);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = parent.ParentId }, parent);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Parent parent)
        {
            if (id != parent.ParentId) return BadRequest();
            _context.Entry(parent).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var parent = await _context.Parents.FindAsync(id);
            if (parent == null) return NotFound();
            _context.Parents.Remove(parent);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
