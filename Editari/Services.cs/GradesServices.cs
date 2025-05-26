using Editari.Data;
using eDitari.Dtos;
using Editari.Models;
using eDitari.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eDitari.Services
{
    public class GradeService(AppDbContext context) : IGradeService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<GradeDto>> GetAllGradesAsync()
        {
            return await _context.Grades
                .Select(g => new GradeDto
                {
                    GradeId = g.GradeId,
                    StudentId = g.StudentId,
                    Subject = g.Subject,
                    GradeValue = g.GradeValue,
                    Date = g.Date
                }).ToListAsync();
        }

        public async Task<GradeDto?> GetGradeByIdAsync(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null) return null;

            return new GradeDto
            {
                GradeId = grade.GradeId,
                StudentId = grade.StudentId,
                Subject = grade.Subject,
                GradeValue = grade.GradeValue,
                Date = grade.Date
            };
        }

        public async Task<GradeDto> CreateGradeAsync(CreateGradeDto gradeDto)
        {
            var grade = new Grade
            {
                StudentId = gradeDto.StudentId,
                Subject = gradeDto.Subject,
                GradeValue = gradeDto.GradeValue,
                Date = gradeDto.Date
            };

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            return new GradeDto
            {
                GradeId = grade.GradeId,
                StudentId = grade.StudentId,
                Subject = grade.Subject,
                GradeValue = grade.GradeValue,
                Date = grade.Date
            };
        }

        public async Task<bool> UpdateGradeAsync(int id, CreateGradeDto gradeDto)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null) return false;

            grade.StudentId = gradeDto.StudentId;
            grade.Subject = gradeDto.Subject;
            grade.GradeValue = gradeDto.GradeValue;
            grade.Date = gradeDto.Date;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteGradeAsync(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null) return false;

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}