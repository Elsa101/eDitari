using eDitari.Dtos;
using Editari.Models;
using eDitari.Services.Interfaces;
using Editari.Data;
using Microsoft.EntityFrameworkCore;

namespace eDitari.Services
{
    public class ParentService : IParentService
    {
        private readonly AppDbContext _context;

        public ParentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ParentDTO>> GetAllParentsAsync()
        {
            return await _context.Parents
                .Select(p => new ParentDTO
                {
                    ParentId = p.ParentId,
                    FullName = p.Name + " " + p.Surname,
                    Email = p.Email,
                    Phone = p.Phone
                })
                .ToListAsync();
        }

        public async Task<ParentDTO?> GetParentByIdAsync(int id)
        {
            var p = await _context.Parents.FindAsync(id);
            if (p == null) return null;

            return new ParentDTO
            {
                ParentId = p.ParentId,
                FullName = p.Name + " " + p.Surname,
                Email = p.Email,
                Phone = p.Phone
            };
        }

        public async Task AddParentAsync(CreateParentDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Emri nuk mund të jetë bosh.");
            if (string.IsNullOrWhiteSpace(dto.Surname))
                throw new ArgumentException("Mbiemri nuk mund të jetë bosh.");
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email-i nuk mund të jetë bosh.");
            if (string.IsNullOrWhiteSpace(dto.Phone))
                throw new ArgumentException("Numri i telefonit nuk mund të jetë bosh.");

            var emailExists = await _context.Parents.AnyAsync(p => p.Email == dto.Email);
            if (emailExists)
                throw new ArgumentException("Ky email është i regjistruar tashmë.");

            var phoneExists = await _context.Parents.AnyAsync(p => p.Phone == dto.Phone);
            if (phoneExists)
                throw new ArgumentException("Ky numër telefoni është i regjistruar tashmë.");

            var parent = new Parent
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                Phone = dto.Phone
            };

            _context.Parents.Add(parent);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateParentAsync(int id, CreateParentDTO dto)
        {
            var parent = await _context.Parents.FindAsync(id);
            if (parent == null) return false;

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Emri nuk mund të jetë bosh.");
            if (string.IsNullOrWhiteSpace(dto.Surname))
                throw new ArgumentException("Mbiemri nuk mund të jetë bosh.");
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email-i nuk mund të jetë bosh.");
            if (string.IsNullOrWhiteSpace(dto.Phone))
                throw new ArgumentException("Numri i telefonit nuk mund të jetë bosh.");

            var emailExists = await _context.Parents.AnyAsync(p => p.Email == dto.Email && p.ParentId != id);
            if (emailExists)
                throw new ArgumentException("Ky email është i përdorur nga një tjetër prind.");

            var phoneExists = await _context.Parents.AnyAsync(p => p.Phone == dto.Phone && p.ParentId != id);
            if (phoneExists)
                throw new ArgumentException("Ky numër telefoni është i përdorur nga një tjetër prind.");

            parent.Name = dto.Name;
            parent.Surname = dto.Surname;
            parent.Email = dto.Email;
            parent.Phone = dto.Phone;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteParentAsync(int id)
        {
            var parent = await _context.Parents.FindAsync(id);
            if (parent == null) return false;

            var hasChildren = await _context.StudentParents.AnyAsync(sp => sp.ParentId == id);
            if (hasChildren)
                throw new InvalidOperationException("Ky prind ka fëmijë të lidhur dhe nuk mund të fshihet.");

            _context.Parents.Remove(parent);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
