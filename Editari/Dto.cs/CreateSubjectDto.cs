using System.ComponentModel.DataAnnotations;

namespace eDitari.Dtos
{
    public class CreateSubjectDto
    {
        [Required(ErrorMessage = "Emri i lëndës është i detyrueshëm.")]
        public required string Name { get; set; }

        [Range(1, 12, ErrorMessage = "Klasa duhet të jetë nga 1 deri në 12.")]
        public int GradeLevel { get; set; }

        [Required(ErrorMessage = "TeacherId është i detyrueshëm.")]
        public int TeacherId { get; set; }
    }
}
