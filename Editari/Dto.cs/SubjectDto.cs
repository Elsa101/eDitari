namespace eDitari.Dtos
{
    public class SubjectDto
    {
        public int SubjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GradeLevel { get; set; }
        public int TeacherId { get; set; }
        public string? TeacherName { get; set; } // nëse përdor .Include në query
        public object? Id { get; internal set; }
        public object? Emri { get; internal set; }
    }
}
