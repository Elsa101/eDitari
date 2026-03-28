using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
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
        public string LinkCode { get; set; } = string.Empty;
 
        public int? ClassId { get; set; }
        public SchoolClass? Class { get; set; }

        public ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
    }
 
    public class Parent
    {
        public int ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
 
        public string PasswordHash { get; set; } = string.Empty;
 
        public ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
    }
 
    [Table("StudentParent")]
    public class StudentParent
    {
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
 
        public int ParentId { get; set; }
        public Parent Parent { get; set; } = null!;
    }
 
    public class Grade
    {
        public int GradeId { get; set; }
        public int StudentId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public decimal GradeValue { get; set; }
        public DateTime Date { get; set; }
    }
 
    [Table("Attendance")]
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public required string Status { get; set; }
    }
 
    public class Teacher
    {
        public int TeacherId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        
        // Login credentials
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
 
    public class Subject
    {
        public int SubjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TeacherId { get; set; }
    }
 
    public class Comment
    {
        public int CommentId { get; set; }
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
 
    public class Staff
    {
        public int StaffId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public int? ClassId { get; set; }
        public SchoolClass? Class { get; set; }
    }

    public class SchoolClass
    {
        [Key]
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;

        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Staff> StaffMembers { get; set; } = new List<Staff>();
    }
 
  
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; } // primary key
 
        public int? ParentId { get; set; }
        public int? StaffId { get; set; }
        public int? TeacherId { get; set; }
 
        [Required]
        public string Token { get; set; } = string.Empty;
 
        public DateTime ExpiresAt { get; set; }
 
        public bool IsRevoked { get; set; } = false;
 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}