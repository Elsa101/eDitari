using Editari.Data;
using eDitari.Dtos;
using Editari.Models;
using eDitari.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eDitari.Services
{
    public class CommentService(AppDbContext context) : ICommentsService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<CommentDto>> GetAllCommentsAsync()
        {
            return await _context.Comments
                .Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    StudentId = c.StudentId,
                    TeacherId = c.TeacherId,
                    CommentText = c.CommentText,
                    Date = c.Date,
                    // Optional: if you want to include related names with .Include
                    // TeacherName = c.Teacher.Name,
                    // StudentName = c.Student.Name
                }).ToListAsync();
        }

        public async Task<CommentDto?> GetCommentByIdAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return null;

            return new CommentDto
            {
                CommentId = comment.CommentId,
                StudentId = comment.StudentId,
                TeacherId = comment.TeacherId,
                CommentText = comment.CommentText,
                Date = comment.Date,
                // TeacherName = comment.Teacher?.Name,
                // StudentName = comment.Student?.Name
            };
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentDto commentDto)
        {
            var comment = new Comment
            {
                StudentId = commentDto.StudentId,
                TeacherId = commentDto.TeacherId,
                CommentText = commentDto.CommentText,
                Date = commentDto.Date
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return new CommentDto
            {
                CommentId = comment.CommentId,
                StudentId = comment.StudentId,
                TeacherId = comment.TeacherId,
                CommentText = comment.CommentText,
                Date = comment.Date
            };
        }

        public async Task<bool> UpdateCommentAsync(int id, CreateCommentDto commentDto)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return false;

            comment.StudentId = commentDto.StudentId;
            comment.TeacherId = commentDto.TeacherId;
            comment.CommentText = commentDto.CommentText;
            comment.Date = commentDto.Date;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return false;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
