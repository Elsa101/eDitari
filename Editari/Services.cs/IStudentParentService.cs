using eDitari.Dtos;

namespace eDitari.Services.Interfaces
{
    public interface IStudentParentService
    {
        Task<List<StudentParentDto>> GetAllAsync();
        Task<StudentParentDto?> GetByIdsAsync(int studentId, int parentId);
        Task AddAsync(CreateStudentParentDto dto);
        Task<bool> UpdateAsync(int studentId, int parentId, CreateStudentParentDto dto); // Opsional për skenarë specifikë
        Task<bool> DeleteAsync(int studentId, int parentId);
    }
}
