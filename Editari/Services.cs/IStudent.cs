using eDitari.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eDitari.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDto>> GetAllStudentsAsync();
        Task<StudentDto?> GetStudentByIdAsync(int id);
        Task<StudentDto> CreateStudentAsync(CreateStudentDto studentDto);
        Task<bool> UpdateStudentAsync(int id, CreateStudentDto studentDto);
        Task<bool> DeleteStudentAsync(int id);
    }
}
