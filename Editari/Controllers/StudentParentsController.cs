using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using Editari.Data;

using Editari.Models;
 
namespace Editari.Controllers

{

    [ApiController]

    [Route("api/[controller]")]

    [Authorize(Roles = "Staff")]

    public class StudentParentsController : ControllerBase

    {

        private readonly AppDbContext _context;
 
        public StudentParentsController(AppDbContext context)

        {

            _context = context;

        }
 
        public class StudentParentCreateDto

        {

            public int StudentId { get; set; }

            public int ParentId { get; set; }

        }
 
        [HttpGet]

        public async Task<ActionResult<IEnumerable<StudentParent>>> GetAll()

        {

            return await _context.StudentParents.ToListAsync();

        }
 
        [HttpPost]

        public async Task<IActionResult> Create([FromBody] StudentParentCreateDto dto)

        {

            var studentExists = await _context.Students.AnyAsync(s => s.StudentId == dto.StudentId);

            if (!studentExists) return BadRequest("Student not found.");
 
            var parentExists = await _context.Parents.AnyAsync(p => p.ParentId == dto.ParentId);

            if (!parentExists) return BadRequest("Parent not found.");
 
            var alreadyLinked = await _context.StudentParents

                .AnyAsync(sp => sp.StudentId == dto.StudentId && sp.ParentId == dto.ParentId);
 
            if (alreadyLinked) return BadRequest("This parent is already linked to this student.");
 
            var link = new StudentParent

            {

                StudentId = dto.StudentId,

                ParentId = dto.ParentId

            };
 
            _context.StudentParents.Add(link);

            await _context.SaveChangesAsync();
 
            return Ok(link);

        }
 
        [HttpDelete]

        public async Task<IActionResult> Delete([FromQuery] int studentId, [FromQuery] int parentId)

        {

            var link = await _context.StudentParents

                .FirstOrDefaultAsync(sp => sp.StudentId == studentId && sp.ParentId == parentId);
 
            if (link == null) return NotFound();
 
            _context.StudentParents.Remove(link);

            await _context.SaveChangesAsync();
 
            return NoContent();

        }

    }

}

 