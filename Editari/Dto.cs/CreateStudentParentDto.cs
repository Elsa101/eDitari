using System;
using System.ComponentModel.DataAnnotations;

namespace eDitari.Dtos
{
    public class CreateStudentParentDto
    {
        [Required(ErrorMessage = "ID e studentit është e detyrueshme.")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "ID e prindit është e detyrueshme.")]
        public int ParentId { get; set; }
    }
}
