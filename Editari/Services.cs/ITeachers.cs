using eDitari.Dtos;

namespace eDitari.Services.Interfaces
{
    public interface ITeacherService
    {
        Task<List<TeacherDTO>> GetAllTeachersAsync();
        Task<TeacherDTO?> GetTeacherByIdAsync(int id);
        Task AddTeacherAsync(CreateTeacherDTO dto);
        Task<bool> UpdateTeacherAsync(int id, CreateTeacherDTO dto); 
        Task<bool> DeleteTeacherAsync(int id);
    }
}
