using System;

namespace eDitari.Dtos
{
    public class GradeDto
    {
        public int GradeId { get; set; }
        public int StudentId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public decimal GradeValue { get; set; }
        public DateTime Date { get; set; }
    }
}
