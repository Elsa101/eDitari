using eDitari.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eDitari.Services.Interfaces
{
    public interface ICommentsService
    {
        Task<IEnumerable<CommentDto>> GetAllCommentsAsync();
        Task<CommentDto?> GetCommentByIdAsync(int id);
        Task<CommentDto> CreateCommentAsync(CreateCommentDto commentDto);
        Task<bool> UpdateCommentAsync(int id, CreateCommentDto commentDto);
        Task<bool> DeleteCommentAsync(int id);
    }
}