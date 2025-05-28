using System.ComponentModel.DataAnnotations;

namespace eDitari.Dtos
{
    public class CreateStaffDto
    {
        [Required(ErrorMessage = "Emri është i detyrueshëm.")]
        [StringLength(100, ErrorMessage = "Emri nuk mund të jetë më i gjatë se 100 karaktere.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Roli është i detyrueshëm.")]
        [StringLength(50, ErrorMessage = "Roli nuk mund të jetë më i gjatë se 50 karaktere.")]
        public required string Role { get; set; }

        [Required(ErrorMessage = "Username është i detyrueshëm.")]
        [StringLength(100, ErrorMessage = "Username nuk mund të jetë më i gjatë se 100 karaktere.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "PasswordHash është i detyrueshëm.")]
        [StringLength(255, ErrorMessage = "PasswordHash nuk mund të jetë më i gjatë se 255 karaktere.")]
        public required string PasswordHash { get; set; }
    }
}
