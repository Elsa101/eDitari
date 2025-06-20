using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Editari.Data;
using Editari.Models;
using eDitari.Dtos; // Add this to use RegisterStaffDto
using BCrypt.Net;   // For password hashing

namespace Editari.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StaffController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Staff>>> GetAll()
        {
            return await _context.Staff.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Staff>> Get(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null) return NotFound();
            return staff;
        }

        [HttpPost]
        public async Task<ActionResult<Staff>> Post(Staff staff)
        {
            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = staff.StaffId }, staff);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Staff staff)
        {
            if (id != staff.StaffId) return BadRequest();
            _context.Entry(staff).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null) return NotFound();
            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ NEW REGISTRATION ENDPOINT
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterStaffDto dto)
        {
            if (await _context.Staff.AnyAsync(s => s.Username == dto.Username))
            {
                return BadRequest("Username already exists.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newStaff = new Staff
            {
                Name = dto.Name,
                Role = dto.Role,
                Username = dto.Username,
                PasswordHash = passwordHash
            };

            _context.Staff.Add(newStaff);
            await _context.SaveChangesAsync();

            return Ok("Registration successful.");
        }
    }
}
