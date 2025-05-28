using eDitari.Dtos;
using Editari.Models;
using eDitari.Services.Interfaces;
using Editari.Data;
using Microsoft.EntityFrameworkCore;

namespace eDitari.Services
{
    public class StaffService : IStaffService
    {
        private readonly AppDbContext _context;

        public StaffService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StaffDto>> GetAllStaffAsync()
        {
            return await _context.Staff
                .Select(s => new StaffDto
                {
                    StaffId = s.StaffId,
                    Name = s.Name,
                    Role = s.Role,
                    Username = s.Username
                    // Do not expose PasswordHash in DTO for security
                })
                .ToListAsync();
        }

        public async Task<StaffDto?> GetStaffByIdAsync(int id)
        {
            var s = await _context.Staff.FindAsync(id);
            if (s == null) return null;

            return new StaffDto
            {
                StaffId = s.StaffId,
                Name = s.Name,
                Role = s.Role,
                Username = s.Username
            };
        }

        public async Task AddStaffAsync(CreateStaffDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Emri nuk mund të jetë bosh.");
            if (string.IsNullOrWhiteSpace(dto.Role))
                throw new ArgumentException("Roli nuk mund të jetë bosh.");
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ArgumentException("Username nuk mund të jetë bosh.");
            if (string.IsNullOrWhiteSpace(dto.PasswordHash))
                throw new ArgumentException("PasswordHash nuk mund të jetë bosh.");

            var usernameExists = await _context.Staff.AnyAsync(s => s.Username == dto.Username);
            if (usernameExists)
                throw new ArgumentException("Ky username është i regjistruar tashmë.");

            var staff = new Staff
            {
                Name = dto.Name,
                Role = dto.Role,
                Username = dto.Username,
                PasswordHash = dto.PasswordHash
            };

            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateStaffAsync(int id, CreateStaffDto dto)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null) return false;

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Emri nuk mund të jetë bosh.");
            if (string.IsNullOrWhiteSpace(dto.Role))
                throw new ArgumentException("Roli nuk mund të jetë bosh.");
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ArgumentException("Username nuk mund të jetë bosh.");
            if (string.IsNullOrWhiteSpace(dto.PasswordHash))
                throw new ArgumentException("PasswordHash nuk mund të jetë bosh.");

            var usernameExists = await _context.Staff.AnyAsync(s => s.Username == dto.Username && s.StaffId != id);
            if (usernameExists)
                throw new ArgumentException("Ky username është i përdorur nga një tjetër staf.");

            staff.Name = dto.Name;
            staff.Role = dto.Role;
            staff.Username = dto.Username;
            staff.PasswordHash = dto.PasswordHash;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null) return false;

            // You can add additional checks here if needed before deletion

            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
