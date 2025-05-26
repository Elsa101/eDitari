using System;
using System.ComponentModel.DataAnnotations;

namespace eDitari.Dtos
{
    public class CreateAttendanceDto
    {
        [Required(ErrorMessage = "StudentId është i detyrueshëm.")]
        public required int StudentId { get; set; }

        [Required(ErrorMessage = "Data është e detyrueshme.")]
        [DataType(DataType.Date, ErrorMessage = "Formati i datës nuk është i vlefshëm.")]
        public required DateTime Date { get; set; }

        [Required(ErrorMessage = "Statusi është i detyrueshëm.")]
        [StringLength(50, ErrorMessage = "Statusi nuk mund të jetë më i gjatë se 50 karaktere.")]
        public required string Status { get; set; }
    }
}