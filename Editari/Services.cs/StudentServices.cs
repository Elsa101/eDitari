using Editari.Data;
using eDitari.Dtos;
using Editari.Models;
using eDitari.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eDitari.Services
{
    public class StudentService(AppDbContext context) : IStudentService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Select(s => new StudentDto
                {
                    StudentId = s.StudentId,
                    Name = s.Name,
                    Surname = s.Surname,
                    DateOfBirth = s.DateOfBirth,
                    Email = s.Email,
                    Phone = s.Phone,
                    Address = s.Address
                }).ToListAsync();
        }

        public async Task<StudentDto?> GetStudentByIdAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return null;

            return new StudentDto
            {
                StudentId = student.StudentId,
                Name = student.Name,
                Surname = student.Surname,
                DateOfBirth = student.DateOfBirth,
                Email = student.Email,
                Phone = student.Phone,
                Address = student.Address
            };
        }

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto studentDto)
        {
            var student = new Student
            {
                Name = studentDto.Name,
                Surname = studentDto.Surname,
                DateOfBirth = studentDto.DateOfBirth,
                Email = studentDto.Email,
                Phone = studentDto.Phone,
                Address = studentDto.Address ?? string.Empty
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return new StudentDto
            {
                StudentId = student.StudentId,
                Name = student.Name,
                Surname = student.Surname,
                DateOfBirth = student.DateOfBirth,
                Email = student.Email,
                Phone = student.Phone,
                Address = student.Address
            };
        }

        public async Task<bool> UpdateStudentAsync(int id, CreateStudentDto studentDto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return false;

            student.Name = studentDto.Name;
            student.Surname = studentDto.Surname;
            student.DateOfBirth = studentDto.DateOfBirth;
            student.Email = studentDto.Email;
            student.Phone = studentDto.Phone;
            student.Address = studentDto.Address ?? string.Empty;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
