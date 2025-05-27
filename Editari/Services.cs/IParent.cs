using eDitari.Dtos;

namespace eDitari.Services.Interfaces
{
    public interface IParentService
    {
        Task<List<ParentDTO>> GetAllParentsAsync();
        Task<ParentDTO?> GetParentByIdAsync(int id);
        Task AddParentAsync(CreateParentDTO dto);
        Task<bool> UpdateParentAsync(int id, CreateParentDTO dto); 
        Task<bool> DeleteParentAsync(int id);
    }
}
