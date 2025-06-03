namespace LanguageCenter.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public string StudentId { get; set; } = null!;
        public ApplicationUser Student { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public DateTime EnrollDate { get; set; } = DateTime.Now;

        public string? Status { get; set; } // Pending, Confirmed, Completed
    }
}
