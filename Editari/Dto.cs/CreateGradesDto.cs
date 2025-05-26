using System;
using System.ComponentModel.DataAnnotations;

namespace eDitari.Dtos
{
    public class CreateGradeDto
    {
        [Required(ErrorMessage = "StudentId është i detyrueshëm.")]
        public required int StudentId { get; set; }

        [Required(ErrorMessage = "Lënda është e detyrueshme.")]
        [StringLength(100, ErrorMessage = "Lënda nuk mund të jetë më e gjatë se 100 karaktere.")]
        public required string Subject { get; set; }

        [Required(ErrorMessage = "Vlera e notës është e detyrueshme.")]
        [Range(0, 100, ErrorMessage = "Nota duhet të jetë nga 1 deri në 5.")]
        public required decimal GradeValue { get; set; }

        [Required(ErrorMessage = "Data është e detyrueshme.")]
        [DataType(DataType.Date, ErrorMessage = "Formati i datës nuk është i vlefshëm.")]
        public required DateTime Date { get; set; }
    }
}
