using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Editari.Data;
using Editari.Models;

namespace Editari.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentParentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentParentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentParent>>> GetAll()
        {
            return await _context.StudentParents.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<StudentParent>> Post(StudentParent studentParent)
        {
            _context.StudentParents.Add(studentParent);
            await _context.SaveChangesAsync();
            return Ok(studentParent);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int studentId, int parentId)
        {
            var entity = await _context.StudentParents
                .FirstOrDefaultAsync(sp => sp.StudentId == studentId && sp.ParentId == parentId);

            if (entity == null) return NotFound();

            _context.StudentParents.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}