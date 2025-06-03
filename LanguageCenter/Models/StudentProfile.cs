using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanguageCenter.Models
{
    public class StudentProfile
    {
        [Key]
        public int Id { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        public string? ParentContact { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;
    }
}
