using eDitari.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eDitari.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceDto>> GetAllAttendancesAsync();
        Task<AttendanceDto?> GetAttendanceByIdAsync(int id);
        Task<AttendanceDto> CreateAttendanceAsync(CreateAttendanceDto attendanceDto);
        Task<bool> UpdateAttendanceAsync(int id, CreateAttendanceDto attendanceDto);
        Task<bool> DeleteAttendanceAsync(int id);
    }
}