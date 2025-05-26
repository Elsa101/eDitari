using System;
using System.ComponentModel.DataAnnotations;

namespace eDitari.Dtos
{
    public class CreateStudentDto
    {
        [Required(ErrorMessage = "Emri është i detyrueshëm.")]
        [StringLength(100, ErrorMessage = "Emri nuk mund të jetë më i gjatë se 100 karaktere.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Mbiemri është i detyrueshëm.")]
        [StringLength(100, ErrorMessage = "Mbiemri nuk mund të jetë më i gjatë se 100 karaktere.")]
        public required string Surname { get; set; }

        [Required(ErrorMessage = "Data e lindjes është e detyrueshme.")]
        [DataType(DataType.Date, ErrorMessage = "Formati i datës nuk është i vlefshëm.")]
        public required DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Emaili është i detyrueshëm.")]
        [EmailAddress(ErrorMessage = "Emaili nuk është në format të vlefshëm.")]
        [StringLength(100, ErrorMessage = "Emaili nuk mund të jetë më i gjatë se 100 karaktere.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Numri i telefonit është i detyrueshëm.")]
        [StringLength(50, ErrorMessage = "Numri i telefonit nuk mund të jetë më i gjatë se 50 karaktere.")]
        public required string Phone { get; set; }

        [StringLength(255, ErrorMessage = "Adresa nuk mund të jetë më e gjatë se 255 karaktere.")]
        public string? Address { get; set; }
    }
}
