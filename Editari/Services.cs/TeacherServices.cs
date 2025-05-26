using eDitari.Dtos;
using Editari.Models;
using eDitari.Services.Interfaces;
using Editari.Data;
using Microsoft.EntityFrameworkCore;


namespace eDitari.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly AppDbContext _context;

        public TeacherService(AppDbContext context)
        {
            _context = context;
        }

        // Merr të gjithë mësuesit
        public async Task<List<TeacherDTO>> GetAllTeachersAsync()
        {
            return await _context.Teachers
                .Select(t => new TeacherDTO
                {
                    TeacherId = t.TeacherId,
                    FullName = t.Name + " " + t.Surname,
                    Email = t.Email,
                    Phone = t.Phone
                })
                .ToListAsync();
        }

        // Merr mësues sipas ID-së
        public async Task<TeacherDTO?> GetTeacherByIdAsync(int id)
        {
            var t = await _context.Teachers.FindAsync(id);
            if (t == null) return null;

            return new TeacherDTO
            {
                TeacherId = t.TeacherId,
                FullName = t.Name + " " + t.Surname,
                Email = t.Email,
                Phone = t.Phone
            };
        }

        // Shto një mësues të ri
        public async Task AddTeacherAsync(CreateTeacherDTO dto)
        {
            var teacher = new Teacher
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                Phone = dto.Phone
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
        }

        // Përditëso një mësues ekzistues
        public async Task<bool> UpdateTeacherAsync(int id, CreateTeacherDTO dto) // përdorim CreateTeacherDTO për të mos e duplikuar
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return false;

            teacher.Name = dto.Name;
            teacher.Surname = dto.Surname;
            teacher.Email = dto.Email;
            teacher.Phone = dto.Phone;

            await _context.SaveChangesAsync();
            return true;
        }

        // Fshi një mësues sipas ID-së
        public async Task<bool> DeleteTeacherAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return false;

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
