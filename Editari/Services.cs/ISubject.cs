using eDitari.Dtos;

namespace eDitari.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync();
        Task<SubjectDto?> GetSubjectByIdAsync(int id);
        Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto subjectDto);
        Task<bool> UpdateSubjectAsync(int id, CreateSubjectDto subjectDto);
        Task<bool> DeleteSubjectAsync(int id);
    }
}
