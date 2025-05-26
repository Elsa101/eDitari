using eDitari.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eDitari.Services.Interfaces
{
    public interface IGradeService
    {
        Task<IEnumerable<GradeDto>> GetAllGradesAsync();
        Task<GradeDto?> GetGradeByIdAsync(int id);
        Task<GradeDto> CreateGradeAsync(CreateGradeDto gradeDto);
        Task<bool> UpdateGradeAsync(int id, CreateGradeDto gradeDto);
        Task<bool> DeleteGradeAsync(int id);
    }
}