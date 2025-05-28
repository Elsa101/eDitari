using Editari.Data;
using eDitari.Dtos;
using Editari.Models;
using eDitari.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eDitari.Services
{
    public class StudentParentService(AppDbContext context) : IStudentParentService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<StudentParentDto>> GetAllAsync()
        {
            return await _context.StudentParents
                .Include(sp => sp.Student)
                .Include(sp => sp.Parent)
                .Select(sp => new StudentParentDto
                {
                    StudentId = sp.StudentId,
                    ParentId = sp.ParentId,
                    StudentName = sp.Student.Name,
                    ParentName = sp.Parent.Name
                }).ToListAsync();
        }

        public async Task<StudentParentDto?> GetByIdsAsync(int studentId, int parentId)
        {
            var sp = await _context.StudentParents
                .Include(x => x.Student)
                .Include(x => x.Parent)
                .FirstOrDefaultAsync(x => x.StudentId == studentId && x.ParentId == parentId);

            if (sp == null) return null;

            return new StudentParentDto
            {
                StudentId = sp.StudentId,
                ParentId = sp.ParentId,
                StudentName = sp.Student.Name,
                ParentName = sp.Parent.Name
            };
        }

        public async Task<StudentParentDto> CreateAsync(CreateStudentParentDto dto)
        {
            var exists = await _context.StudentParents
                .AnyAsync(x => x.StudentId == dto.StudentId && x.ParentId == dto.ParentId);
            if (exists)
                throw new InvalidOperationException("Kjo lidhje tashmë ekziston.");

            var sp = new StudentParent
            {
                StudentId = dto.StudentId,
                ParentId = dto.ParentId
            };

            _context.StudentParents.Add(sp);
            await _context.SaveChangesAsync();

            return new StudentParentDto
            {
                StudentId = sp.StudentId,
                ParentId = sp.ParentId
            };
        }

        public async Task<bool> DeleteAsync(int studentId, int parentId)
        {
            var sp = await _context.StudentParents.FindAsync(studentId, parentId);
            if (sp == null) return false;

            _context.StudentParents.Remove(sp);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
