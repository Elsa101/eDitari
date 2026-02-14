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
    [Authorize(Roles = "Staff")]
    public class SubjectsController : ControllerBase
    {
        private readonly AppDbContext _context;
 
        public SubjectsController(AppDbContext context)
        {
            _context = context;
        }
 
        // ✅ Diagnostic endpoint (vetëm për testim)
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
        public async Task<ActionResult<IEnumerable<Subject>>> GetAll()
        {
            return await _context.Subjects.ToListAsync();
        }
 
        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> Get(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound();
 
            return subject;
        }
 
        [HttpPost]
        public async Task<ActionResult<Subject>> Post([FromBody] Subject subject)
        {
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
 
            return CreatedAtAction(nameof(Get), new { id = subject.SubjectId }, subject);
        }
 
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Subject subject)
        {
            if (id != subject.SubjectId)
                return BadRequest();
 
            _context.Entry(subject).State = EntityState.Modified;
            await _context.SaveChangesAsync();
 
            return NoContent();
        }
 
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound();
 
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
 
            
        }
    }
}