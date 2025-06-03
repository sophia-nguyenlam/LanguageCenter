using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanguageCenter.Models
{
    public class TeacherProfile
    {
        [Key]
        public int Id { get; set; }

        public string Expertise { get; set; } = string.Empty;

        public int YearsOfExperience { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;
    }
}
