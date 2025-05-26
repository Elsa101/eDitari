using System;

namespace eDitari.Dtos
{
    public class AttendanceDto
    {
        public int AttendanceId { get; set; }

        public required int StudentId { get; set; }
        public required DateTime Date { get; set; }
        public required string Status { get; set; }

   
        public StudentDto? Student { get; set; }
    }
}