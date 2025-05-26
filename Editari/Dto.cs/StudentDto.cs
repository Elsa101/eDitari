namespace eDitari.Dtos
{
    public class StudentDto
    {
        public int StudentId { get; set; }

        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public string? Address { get; set; }

        
        public object? Id { get; internal set; }
        public object? Emri { get; internal set; }
    }
}
