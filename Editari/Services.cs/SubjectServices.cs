using Editari.Data;
using eDitari.Dtos;
using Editari.Models;
using eDitari.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using editariAPI.Models;

namespace eDitari.Services
{
    public class SubjectService(AppDbContext context) : ISubjectService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync()
        {
            return await _context.Subjects
                .Select(static s => new SubjectDto
                {
                    Name = s.Name
                }).ToListAsync();
        }

        public async Task<SubjectDto?> GetSubjectByIdAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return null;

            return new SubjectDto
            {
                SubjectId = subject.SubjectId,
                Name = subject.Name
            };
        }

        public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto subjectDto)
        {
            var subject = new Editari.Models.Subject
            {
                Name = subjectDto.Name
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return new SubjectDto
            {
                SubjectId = subject.SubjectId,
                Name = subject.Name
            };
        }

        public async Task<bool> UpdateSubjectAsync(int id, CreateSubjectDto subjectDto)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return false;

            subject.Name = subjectDto.Name;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return false;

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
