namespace Editari.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
namespace Editari.Models
{
    public class Grade
    {
        public int GradeId { get; set; }
        public int StudentId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public decimal GradeValue { get; set; }
        public DateTime Date { get; set; }
    }
}
namespace Editari.Models
{
  public class Attendance
    {
        public int AttendanceId { get; set; }
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public required string Status { get; set; }
    }
}
