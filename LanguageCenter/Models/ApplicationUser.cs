using LanguageCenter.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LanguageCenter.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        [Required]
        public string Role { get; set; } = UserRoles.Student;

        // Navigation 1-1
        public StudentProfile? StudentProfile { get; set; }
        public TeacherProfile? TeacherProfile { get; set; }

        // Navigation 1-n
        public ICollection<ClassSession>? ClassSessionsAsTeacher { get; set; }
        public ICollection<Attendance>? AttendancesAsStudent { get; set; }
    }


}
