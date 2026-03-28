using Microsoft.AspNetCore.Mvc;
using Editari.Data;
using Microsoft.EntityFrameworkCore;
using Editari.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

                                         
namespace Editari.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public class StudentCreateDto
        {
            public int StudentId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public DateTime DateOfBirth { get; set; }
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public int? ClassId { get; set; }
            public int? ParentId { get; set; }
        }

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("secure-test")]
        public IActionResult SecureTest()
{
        return Ok("🎉 Authorized! Token works.");
}


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAll()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userRole == "Admin")
            {
                return await _context.Students.ToListAsync();
            }

            if (userRole == "Staff" && !string.IsNullOrEmpty(userIdStr))
            {
                var staffId = int.Parse(userIdStr);
                var staff = await _context.Staff.FindAsync(staffId);
                
                if (staff == null || !staff.ClassId.HasValue)
                {
                    return Ok(new List<Student>());
                }

                return await _context.Students
                    .Where(s => s.ClassId == staff.ClassId.Value)
                    .ToListAsync();
            }

            return await _context.Students.ToListAsync();
        }

        // ── Nxënësit me prindërit e lidhur (për Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet("with-parents")]
        public async Task<IActionResult> GetWithParents()
        {
            var result = await _context.Students
                .Select(s => new
                {
                    s.StudentId,
                    s.Name,
                    s.Surname,
                    s.Email,
                    s.Phone,
                    s.Address,
                    s.ClassId,
                    s.LinkCode,
                    Parents = _context.StudentParents
                        .Where(sp => sp.StudentId == s.StudentId)
                        .Select(sp => new { sp.Parent.ParentId, sp.Parent.Name, sp.Parent.Surname, sp.Parent.Email, sp.Parent.Phone })
                        .ToList()
                })
                .ToListAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> Get(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();
            return student;
        }

        [Authorize(Roles = "Admin,Teacher,Staff")]
        [HttpPost]
        public async Task<ActionResult<Student>> Post(StudentCreateDto dto)
        {
            var student = new Student
            {
                Name = dto.Name,
                Surname = dto.Surname,
                DateOfBirth = dto.DateOfBirth,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                ClassId = dto.ClassId,
                LinkCode = Guid.NewGuid().ToString().ToUpper().Substring(0, 8)
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            if (dto.ParentId.HasValue)
            {
                var studentParent = new StudentParent
                {
                    StudentId = student.StudentId,
                    ParentId = dto.ParentId.Value
                };
                _context.StudentParents.Add(studentParent);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(Get), new { id = student.StudentId }, student);
        }

        [Authorize(Roles = "Admin,Teacher,Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Student student)
        {
            if (id != student.StudentId) return BadRequest();

            var existingStudent = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == id);
            if (existingStudent == null) return NotFound();

            student.LinkCode = existingStudent.LinkCode; // Ruajmë kodin ekzistues

            _context.Entry(student).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Fshirja brute-force e të gjitha të dhënave të ndërlidhura me StudentId
                await _context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM Grades WHERE StudentId = {id}");
                await _context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM [Attendances] WHERE StudentId = {id}");
                await _context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM Comments WHERE StudentId = {id}");
                await _context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM StudentParent WHERE StudentId = {id}");
                await _context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM Students WHERE StudentId = {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                // Në rast se ka ndonjë gabim, kthejmë mesazhin për debug
                return StatusCode(500, $"Gabim në SQL: {ex.Message}");
            }
        }
        public class AssignClassDto
        {
            public int StudentId { get; set; }
            public string ClassName { get; set; } = string.Empty;
            public int? StaffId { get; set; } // Opcionale: ID e mësuesit për t'u lidhur me këtë klasë
        }

        [Authorize(Roles = "Admin,Teacher,Staff")]
        [HttpPost("assign-class")]
        public async Task<IActionResult> AssignClass([FromBody] AssignClassDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClassName))
                return BadRequest("Emri i klasës nuk mund të jetë bosh.");

            // 1. Gjejmë ose krijojmë klasën
            var schoolClass = await _context.SchoolClasses
                .FirstOrDefaultAsync(c => c.ClassName.ToLower() == dto.ClassName.ToLower());

            if (schoolClass == null)
            {
                schoolClass = new SchoolClass { ClassName = dto.ClassName };
                _context.SchoolClasses.Add(schoolClass);
                await _context.SaveChangesAsync();
            }

            // 2. Lidhim nxënësin me klasën
            var student = await _context.Students.FindAsync(dto.StudentId);
            if (student == null) return NotFound("Nxënësi nuk u gjet.");

            student.ClassId = schoolClass.ClassId;
            _context.Entry(student).State = EntityState.Modified;

            // 3. Nëse është dhënë mësuesi, e lidhim edhe mësuesin me këtë klasë
            if (dto.StaffId.HasValue)
            {
                var staff = await _context.Staff.FindAsync(dto.StaffId.Value);
                if (staff != null)
                {
                    // Admins should never be linked to classes
                    if (staff.Role == "Admin")
                        return BadRequest("Adminët nuk mund të lidhen me klasa.");

                    staff.ClassId = schoolClass.ClassId;
                    _context.Entry(staff).State = EntityState.Modified;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Lidhja u krye me sukses!", className = schoolClass.ClassName });
        }
    }
}
