using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Editari.Data;
using Editari.Models;

namespace Editari.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendancesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttendancesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAll()
        {
            return await _context.Attendances.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Attendance>> Get(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null) return NotFound();
            return attendance;
        }

        [HttpPost]
        public async Task<ActionResult<Attendance>> Post(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = attendance.AttendanceId }, attendance);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Attendance attendance)
        {
            if (id != attendance.AttendanceId) return BadRequest();
            _context.Entry(attendance).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null) return NotFound();
            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
