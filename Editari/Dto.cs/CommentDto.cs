namespace eDitari.Dtos
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        
        // Optional: if you want to include teacher name when using .Include in queries
        public string? TeacherName { get; set; }
        
        // Optional: if you want to include student full name when using .Include in queries
        public string? StudentName { get; set; }
        
        // These look like extra properties from your example; you can omit if not needed
        public object? Id { get; internal set; }
        public object? Emri { get; internal set; }
    }
}
