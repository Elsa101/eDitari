
using System;
using System.ComponentModel.DataAnnotations;

namespace eDitari.Dtos
{
    public class CreateCommentDto
    {
        [Required(ErrorMessage = "StudentId është i detyrueshëm.")]
        public required int StudentId { get; set; }

        [Required(ErrorMessage = "TeacherId është i detyrueshëm.")]
        public required int TeacherId { get; set; }

        [Required(ErrorMessage = "Teksti i komentit është i detyrueshëm.")]
        [StringLength(1000, ErrorMessage = "Komenti nuk mund të jetë më i gjatë se 1000 karaktere.")]
        public required string CommentText { get; set; }

        [Required(ErrorMessage = "Data është e detyrueshme.")]
        [DataType(DataType.Date, ErrorMessage = "Formati i datës nuk është i vlefshëm.")]
        public required DateTime Date { get; set; }
    }
}
