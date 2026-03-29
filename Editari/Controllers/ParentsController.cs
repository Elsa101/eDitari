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
            var normalizedEmail = dto.Email?.Trim().ToLower() ?? string.Empty;

            if (await _context.Parents.AnyAsync(p => p.Email.ToLower() == normalizedEmail))
                return BadRequest("Email already exists.");
 
            var parent = new Parent
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = normalizedEmail,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };
 
            _context.Parents.Add(parent);
            await _context.SaveChangesAsync();
 
            return Ok(new { parent.ParentId, parent.Email });
        }
 
        // ---------------- STAFF CRUD ----------------
 
        [Authorize(Roles = "Admin,Staff,Teacher")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Parent>>> GetAll()
        {
            return await _context.Parents.ToListAsync();
        }
 
        [Authorize(Roles = "Admin,Staff,Teacher")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Parent>> Get(int id)
        {
            var parent = await _context.Parents.FindAsync(id);
            if (parent == null) return NotFound();
            return parent;
        }
 
        [Authorize(Roles = "Admin,Staff,Teacher")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Parent parent)
        {
            if (id != parent.ParentId) return BadRequest();
 
            _context.Entry(parent).State = EntityState.Modified;
            await _context.SaveChangesAsync();
 
            return NoContent();
        }
 
        [Authorize(Roles = "Admin,Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var parent = await _context.Parents.FindAsync(id);
            if (parent == null) return NotFound();

            // 1. Remove related RefreshTokens directly
            await _context.RefreshTokens.Where(rt => rt.ParentId == id).ExecuteDeleteAsync();

            // 2. Remove related StudentParent links directly
            await _context.StudentParents.Where(sp => sp.ParentId == id).ExecuteDeleteAsync();

            // 3. Remove the Parent directly from DB to avoid entity tracking state issues
            await _context.Parents.Where(p => p.ParentId == id).ExecuteDeleteAsync();

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
               
 
        [Authorize(Roles = "Parent")]
[HttpGet("my-children/attendance")]
public async Task<IActionResult> MyChildrenAttendance()
{
    var parentIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(parentIdStr))
        return Unauthorized("ParentId missing in token.");
 
    var parentId = int.Parse(parentIdStr);
 
    var studentIds = await _context.StudentParents
        .Where(sp => sp.ParentId == parentId)
        .Select(sp => sp.StudentId)
        .ToListAsync();
 
    var attendance = await _context.Attendances
        .Where(a => studentIds.Contains(a.StudentId))
        .Select(a => new
        {
            a.AttendanceId,
            a.StudentId,
            a.Date,
            a.Status
        })
        .OrderByDescending(a => a.Date)
        .ToListAsync();
 
    return Ok(attendance);
}
 
       
    [Authorize(Roles = "Parent")]
[HttpGet("my-children/comments")]
public async Task<IActionResult> MyChildrenComments()
{
    var parentIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(parentIdStr))
        return Unauthorized("ParentId missing in token.");
 
    var parentId = int.Parse(parentIdStr);
 
    var studentIds = await _context.StudentParents
        .Where(sp => sp.ParentId == parentId)
        .Select(sp => sp.StudentId)
        .ToListAsync();
 
    var comments = await _context.Comments
        .Where(c => studentIds.Contains(c.StudentId))
        .Select(c => new
        {
            c.CommentId,
            c.StudentId,
            c.TeacherId,
            c.CommentText,
            c.Date
        })
        .OrderByDescending(c => c.Date)
        .ToListAsync();
 
    return Ok(comments);
    }

    // ---------------- NEW: LINK STUDENT ----------------

    public class LinkStudentDto
    {
        public int StudentId { get; set; }
        public string LinkCode { get; set; } = string.Empty;
    }

    [Authorize(Roles = "Parent")]
    [HttpPost("link-student")]
    public async Task<IActionResult> LinkStudent([FromBody] LinkStudentDto dto)
    {
        var parentIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(parentIdStr))
            return Unauthorized("ParentId missing in token.");

        var parentId = int.Parse(parentIdStr);

        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == dto.StudentId && s.LinkCode == dto.LinkCode);

        if (student == null)
            return BadRequest("ID-ja e nxënësit ose Kodi i Lidhjes është i pasaktë.");

        var alreadyLinked = await _context.StudentParents
            .AnyAsync(sp => sp.StudentId == dto.StudentId && sp.ParentId == parentId);

        if (alreadyLinked)
            return BadRequest("Ky fëmijë është tashmë i lidhur me llogarinë tuaj.");

        var link = new StudentParent
        {
            StudentId = dto.StudentId,
            ParentId = parentId
        };

        _context.StudentParents.Add(link);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Fëmija u lidh me sukses!", studentName = $"{student.Name} {student.Surname}" });
    }

    // ---------------- NEW: GET TEACHERS ----------------

    [Authorize(Roles = "Parent")]
    [HttpGet("my-children/teachers")]
    public async Task<IActionResult> GetMyChildrenTeachers()
    {
        var parentIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(parentIdStr))
            return Unauthorized("ParentId missing in token.");

        var parentId = int.Parse(parentIdStr);

        var studentIds = await _context.StudentParents
            .Where(sp => sp.ParentId == parentId)
            .Select(sp => sp.StudentId)
            .ToListAsync();

        // Join Students -> Subjects -> Teachers
        var teachers = await _context.Subjects
            .Where(s => _context.Grades.Any(g => g.StudentId != 0 && studentIds.Contains(g.StudentId) && g.Subject == s.Name) || 
                        _context.Comments.Any(c => studentIds.Contains(c.StudentId) && c.TeacherId == s.TeacherId))
            // Simplified: return all teachers associated with subjects (assuming students have subjects)
            // But subjects don't have a direct link to students in this model, they are linked via TeacherId and maybe Grade.Subject string.
            // Let's get teachers from all subjects for now, or from comments.
            .Select(s => new
            {
                s.TeacherId,
                TeacherName = _context.Teachers.Where(t => t.TeacherId == s.TeacherId).Select(t => t.Name + " " + t.Surname).FirstOrDefault(),
                SubjectName = s.Name,
                Email = _context.Teachers.Where(t => t.TeacherId == s.TeacherId).Select(t => t.Email).FirstOrDefault()
            })
            .Distinct()
            .ToListAsync();

        return Ok(teachers);
    }
    }
}