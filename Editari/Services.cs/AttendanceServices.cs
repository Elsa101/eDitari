using Editari.Data;
using eDitari.Dtos;
using Editari.Models;
using eDitari.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eDitari.Services
{
    public class AttendanceService(AppDbContext context) : IAttendanceService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<AttendanceDto>> GetAllAttendancesAsync()
        {
            return await _context.Attendances
                .Select(a => new AttendanceDto
                {
                    AttendanceId = a.AttendanceId,
                    StudentId = a.StudentId,
                    Date = a.Date,
                    Status = a.Status
                }).ToListAsync();
        }

        public async Task<AttendanceDto?> GetAttendanceByIdAsync(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null) return null;

            return new AttendanceDto
            {
                AttendanceId = attendance.AttendanceId,
                StudentId = attendance.StudentId,
                Date = attendance.Date,
                Status = attendance.Status
            };
        }

        public async Task<AttendanceDto> CreateAttendanceAsync(CreateAttendanceDto attendanceDto)
        {
            var attendance = new Attendance
            {
                StudentId = attendanceDto.StudentId,
                Date = attendanceDto.Date,
                Status = attendanceDto.Status
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return new AttendanceDto
            {
                AttendanceId = attendance.AttendanceId,
                StudentId = attendance.StudentId,
                Date = attendance.Date,
                Status = attendance.Status
            };
        }

        public async Task<bool> UpdateAttendanceAsync(int id, CreateAttendanceDto attendanceDto)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null) return false;

            attendance.StudentId = attendanceDto.StudentId;
            attendance.Date = attendanceDto.Date;
            attendance.Status = attendanceDto.Status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAttendanceAsync(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null) return false;

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
