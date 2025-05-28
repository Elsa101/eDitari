using eDitari.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eDitari.Services.Interfaces
{
    public interface IStaffService
    {
        Task<List<StaffDto>> GetAllStaffAsync();
        Task<StaffDto?> GetStaffByIdAsync(int id);
        Task AddStaffAsync(CreateStaffDto dto);
        Task<bool> UpdateStaffAsync(int id, CreateStaffDto dto);
        Task<bool> DeleteStaffAsync(int id);
    }
}
