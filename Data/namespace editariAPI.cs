namespace editariAPI.Models
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
namespace editariAPI.Models
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
amespace editariAPI.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}

namespace editariAPI.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TeacherId { get; set; }
    }
}

namespace editariAPI.Models
{
    public class Parent
    {
        public int ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}